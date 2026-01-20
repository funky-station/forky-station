// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2025 Milon
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 Henry
// SPDX-FileCopyrightText: 2023 Vyacheslav Kovalevsky
// SPDX-FileCopyrightText: 2023 Chubbygummibear
// SPDX-FileCopyrightText: 2023 Justin Pfeifler
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Júlio César Ueti
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Jacob Tong
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Gravity;
using Content.Shared.Physics;
using Content.Shared.Movement.Pulling.Events;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Throwing
{
    /// <summary>
    ///     Handles throwing landing and collisions.
    /// </summary>
    public sealed class ThrownItemSystem : EntitySystem
    {
        [Dependency] private readonly IGameTiming _gameTiming = default!;
        [Dependency] private readonly INetManager _netMan = default!;
        [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
        [Dependency] private readonly FixtureSystem _fixtures = default!;
        [Dependency] private readonly SharedBroadphaseSystem _broadphase = default!;
        [Dependency] private readonly SharedPhysicsSystem _physics = default!;
        [Dependency] private readonly SharedGravitySystem _gravity = default!;

        private const string ThrowingFixture = "throw-fixture";

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<ThrownItemComponent, MapInitEvent>(OnMapInit);
            SubscribeLocalEvent<ThrownItemComponent, PhysicsSleepEvent>(OnSleep);
            SubscribeLocalEvent<ThrownItemComponent, StartCollideEvent>(HandleCollision);
            SubscribeLocalEvent<ThrownItemComponent, PreventCollideEvent>(PreventCollision);
            SubscribeLocalEvent<ThrownItemComponent, ThrownEvent>(ThrowItem);

            SubscribeLocalEvent<PullStartedMessage>(HandlePullStarted);
        }

        private void OnMapInit(EntityUid uid, ThrownItemComponent component, MapInitEvent args)
        {
            component.ThrownTime ??= _gameTiming.CurTime;
        }

        private void ThrowItem(EntityUid uid, ThrownItemComponent component, ref ThrownEvent @event)
        {
            if (!TryComp(uid, out FixturesComponent? fixturesComponent) ||
                fixturesComponent.Fixtures.Count != 1 ||
                !TryComp<PhysicsComponent>(uid, out var body))
            {
                return;
            }

            var fixture = fixturesComponent.Fixtures.Values.First();
            var shape = fixture.Shape;
            _fixtures.TryCreateFixture(uid, shape, ThrowingFixture, hard: false, collisionMask: (int) CollisionGroup.ThrownItem, manager: fixturesComponent, body: body);
        }

        private void HandleCollision(EntityUid uid, ThrownItemComponent component, ref StartCollideEvent args)
        {
            if (!args.OtherFixture.Hard)
                return;

            if (args.OtherEntity == component.Thrower)
                return;

            ThrowCollideInteraction(component, args.OurEntity, args.OtherEntity);
        }

        private void PreventCollision(EntityUid uid, ThrownItemComponent component, ref PreventCollideEvent args)
        {
            if (args.OtherEntity == component.Thrower)
            {
                args.Cancelled = true;
            }
        }

        private void OnSleep(EntityUid uid, ThrownItemComponent thrownItem, ref PhysicsSleepEvent @event)
        {
            StopThrow(uid, thrownItem);
        }

        private void HandlePullStarted(PullStartedMessage message)
        {
            // TODO: this isn't directed so things have to be done the bad way
            if (TryComp(message.PulledUid, out ThrownItemComponent? thrownItemComponent))
                StopThrow(message.PulledUid, thrownItemComponent);
        }

        public void StopThrow(EntityUid uid, ThrownItemComponent thrownItemComponent)
        {
            if (TryComp<PhysicsComponent>(uid, out var physics))
            {
                _physics.SetBodyStatus(uid, physics, BodyStatus.OnGround);

                if (physics.Awake)
                    _broadphase.RegenerateContacts((uid, physics));
            }

            if (TryComp(uid, out FixturesComponent? manager))
            {
                var fixture = _fixtures.GetFixtureOrNull(uid, ThrowingFixture, manager: manager);

                if (fixture != null)
                {
                    _fixtures.DestroyFixture(uid, ThrowingFixture, fixture, manager: manager);
                }
            }

            var ev = new StopThrowEvent(thrownItemComponent.Thrower);
            RaiseLocalEvent(uid, ref ev);
            RemComp<ThrownItemComponent>(uid);
        }

        public void LandComponent(EntityUid uid, ThrownItemComponent thrownItem, PhysicsComponent physics, bool playSound)
        {
            if (thrownItem.Landed || thrownItem.Deleted || _gravity.IsWeightless(uid) || Deleted(uid))
                return;

            thrownItem.Landed = true;

            // Assume it's uninteresting if it has no thrower. For now anyway.
            if (thrownItem.Thrower is not null)
                _adminLogger.Add(LogType.Landed, LogImpact.Low, $"{ToPrettyString(uid):entity} thrown by {ToPrettyString(thrownItem.Thrower.Value):thrower} landed.");

            _broadphase.RegenerateContacts((uid, physics));
            var landEvent = new LandEvent(thrownItem.Thrower, playSound);
            RaiseLocalEvent(uid, ref landEvent);
        }

        /// <summary>
        ///     Raises collision events on the thrown and target entities.
        /// </summary>
        public void ThrowCollideInteraction(ThrownItemComponent component, EntityUid thrown, EntityUid target)
        {
            if (component.Thrower is not null)
                _adminLogger.Add(LogType.ThrowHit, LogImpact.Low,
                    $"{ToPrettyString(thrown):thrown} thrown by {ToPrettyString(component.Thrower.Value):thrower} hit {ToPrettyString(target):target}.");

            var hitByEv = new ThrowHitByEvent(thrown, target, component);
            var doHitEv = new ThrowDoHitEvent(thrown, target, component);
            RaiseLocalEvent(target, ref hitByEv, true);
            RaiseLocalEvent(thrown, ref doHitEv, true);
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            var query = EntityQueryEnumerator<ThrownItemComponent, PhysicsComponent>();
            while (query.MoveNext(out var uid, out var thrown, out var physics))
            {
                // If you remove this check verify slipping for other entities is networked properly.
                if (_netMan.IsClient && !physics.Predict)
                    continue;

                if (thrown.LandTime <= _gameTiming.CurTime)
                {
                    LandComponent(uid, thrown, physics, thrown.PlayLandSound);
                }

                var stopThrowTime = thrown.LandTime ?? thrown.ThrownTime;
                if (stopThrowTime <= _gameTiming.CurTime)
                {
                    StopThrow(uid, thrown);
                }
            }
        }
    }
}
