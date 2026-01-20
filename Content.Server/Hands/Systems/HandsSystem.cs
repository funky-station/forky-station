// SPDX-FileCopyrightText: 2026 pathetic meowmeow
// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2025 SlamBamActionman
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 ShadowCommander
// SPDX-FileCopyrightText: 2024 Simon
// SPDX-FileCopyrightText: 2024 0x6273
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Callmore
// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 KP
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Menshin
// SPDX-FileCopyrightText: 2023 Tyzemol
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2021 wrexbe
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Fishfish458
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Clyybber
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2021 T-Stalker
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2018 Acruid
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Remie Richards
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-FileCopyrightText: 2020 derek
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Tomeno
// SPDX-FileCopyrightText: 2020 Hugal31
// SPDX-FileCopyrightText: 2020 Injazz
// SPDX-FileCopyrightText: 2020 Profane McBane
// SPDX-FileCopyrightText: 2020 Qustinnus
// SPDX-FileCopyrightText: 2020 Markus W. Halvorsen
// SPDX-FileCopyrightText: 2019 L.E.D
// SPDX-FileCopyrightText: 2019 PrPleGoo
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Server.Stack;
using Content.Server.Stunnable;
using Content.Shared.ActionBlocker;
using Content.Shared.CombatMode;
using Content.Shared.Damage.Systems;
using Content.Shared.Explosion;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Stacks;
using Content.Shared.Standing;
using Content.Shared.Throwing;
using Robust.Shared.GameStates;
using Robust.Shared.Input.Binding;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Hands.Systems
{
    public sealed class HandsSystem : SharedHandsSystem
    {
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly StackSystem _stackSystem = default!;
        [Dependency] private readonly ActionBlockerSystem _actionBlockerSystem = default!;
        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
        [Dependency] private readonly PullingSystem _pullingSystem = default!;
        [Dependency] private readonly ThrowingSystem _throwingSystem = default!;

        private EntityQuery<PhysicsComponent> _physicsQuery;

        /// <summary>
        /// Items dropped when the holder falls down will be launched in
        /// a direction offset by up to this many degrees from the holder's
        /// movement direction.
        /// </summary>
        private const float DropHeldItemsSpread = 45;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<HandsComponent, DisarmedEvent>(OnDisarmed, before: new[] {typeof(StunSystem), typeof(SharedStaminaSystem)});

            SubscribeLocalEvent<HandsComponent, ComponentGetState>(GetComponentState);

            SubscribeLocalEvent<HandsComponent, BeforeExplodeEvent>(OnExploded);

            SubscribeLocalEvent<HandsComponent, DropHandItemsEvent>(OnDropHandItems);

            CommandBinds.Builder
                .Bind(ContentKeyFunctions.ThrowItemInHand, new PointerInputCmdHandler(HandleThrowItem))
                .Register<HandsSystem>();

            _physicsQuery = GetEntityQuery<PhysicsComponent>();
        }

        public override void Shutdown()
        {
            base.Shutdown();

            CommandBinds.Unregister<HandsSystem>();
        }

        private void GetComponentState(EntityUid uid, HandsComponent hands, ref ComponentGetState args)
        {
            args.State = new HandsComponentState(hands);
        }


        private void OnExploded(Entity<HandsComponent> ent, ref BeforeExplodeEvent args)
        {
            if (ent.Comp.DisableExplosionRecursion)
                return;

            foreach (var held in EnumerateHeld(ent.AsNullable()))
            {
                args.Contents.Add(held);
            }
        }

        private void OnDisarmed(EntityUid uid, HandsComponent component, ref DisarmedEvent args)
        {
            if (args.Handled)
                return;

            // Break any pulls
            if (TryComp(uid, out PullerComponent? puller) && TryComp(puller.Pulling, out PullableComponent? pullable))
                _pullingSystem.TryStopPull(puller.Pulling.Value, pullable);

            var offsetRandomCoordinates = _transformSystem.GetMoverCoordinates(args.Target).Offset(_random.NextVector2(1f, 1.5f));
            if (!ThrowHeldItem(args.Target, offsetRandomCoordinates))
                return;

            args.PopupPrefix = "disarm-action-";

            args.Handled = true; // no shove/stun.
        }

        #region interactions

        private bool HandleThrowItem(ICommonSession? playerSession, EntityCoordinates coordinates, EntityUid entity)
        {
            if (playerSession?.AttachedEntity is not {Valid: true} player || !Exists(player) || !coordinates.IsValid(EntityManager))
                return false;

            return ThrowHeldItem(player, coordinates);
        }

        /// <summary>
        /// Throw the player's currently held item.
        /// </summary>
        public bool ThrowHeldItem(EntityUid player, EntityCoordinates coordinates, float minDistance = 0.1f)
        {
            if (ContainerSystem.IsEntityInContainer(player) ||
                !TryComp(player, out HandsComponent? hands) ||
                !TryGetActiveItem((player, hands), out var throwEnt) ||
                !_actionBlockerSystem.CanThrow(player, throwEnt.Value))
                return false;

            if (_timing.CurTime < hands.NextThrowTime)
                return false;
            hands.NextThrowTime = _timing.CurTime + hands.ThrowCooldown;

            if (TryComp(throwEnt, out StackComponent? stack) && stack.Count > 1 && stack.ThrowIndividually)
            {
                var splitStack = _stackSystem.Split((throwEnt.Value, stack), 1, Comp<TransformComponent>(player).Coordinates);

                if (splitStack is not {Valid: true})
                    return false;

                throwEnt = splitStack.Value;
            }

            var direction = _transformSystem.ToMapCoordinates(coordinates).Position - _transformSystem.GetWorldPosition(player);
            if (direction == Vector2.Zero)
                return true;

            var length = direction.Length();
            var distance = Math.Clamp(length, minDistance, hands.ThrowRange);
            direction *= distance / length;

            var throwSpeed = hands.BaseThrowspeed;

            // Let other systems change the thrown entity (useful for virtual items)
            // or the throw strength.
            var ev = new BeforeThrowEvent(throwEnt.Value, direction, throwSpeed, player);
            RaiseLocalEvent(player, ref ev);

            if (ev.Cancelled)
                return true;

            // This can grief the above event so we raise it afterwards
            if (IsHolding((player, hands), throwEnt, out _) && !TryDrop(player, throwEnt.Value))
                return false;

            _throwingSystem.TryThrow(ev.ItemUid, ev.Direction, ev.ThrowSpeed, ev.PlayerUid, compensateFriction: !HasComp<LandAtCursorComponent>(ev.ItemUid));

            return true;
        }

        private void OnDropHandItems(Entity<HandsComponent> entity, ref DropHandItemsEvent args)
        {
            // If the holder doesn't have a physics component, they ain't moving
            var holderVelocity = _physicsQuery.TryComp(entity, out var physics) ? physics.LinearVelocity : Vector2.Zero;
            var spreadMaxAngle = Angle.FromDegrees(DropHeldItemsSpread);

            foreach (var hand in entity.Comp.Hands.Keys)
            {
                if (!TryGetHeldItem(entity.AsNullable(), hand, out var heldEntity))
                    continue;

                var throwAttempt = new FellDownThrowAttemptEvent(entity);
                RaiseLocalEvent(heldEntity.Value, ref throwAttempt);

                if (throwAttempt.Cancelled)
                    continue;

                if (!TryDrop(entity.AsNullable(), hand, checkActionBlocker: false))
                    continue;

                // Rotate the item's throw vector a bit for each item
                var angleOffset = _random.NextAngle(-spreadMaxAngle, spreadMaxAngle);
                // Rotate the holder's velocity vector by the angle offset to get the item's velocity vector
                var itemVelocity = angleOffset.RotateVec(holderVelocity);
                // Decrease the distance of the throw by a random amount
                itemVelocity *= _random.NextFloat(1f);
                // Heavier objects don't get thrown as far
                // If the item doesn't have a physics component, it isn't going to get thrown anyway, but we'll assume infinite mass
                itemVelocity *= _physicsQuery.TryComp(heldEntity, out var heldPhysics) ? heldPhysics.InvMass : 0;
                // Throw at half the holder's intentional throw speed and
                // vary the speed a little to make it look more interesting
                var throwSpeed = entity.Comp.BaseThrowspeed * _random.NextFloat(0.45f, 0.55f);

                _throwingSystem.TryThrow(heldEntity.Value,
                    itemVelocity,
                    throwSpeed,
                    entity,
                    pushbackRatio: 0,
                    compensateFriction: false
                );
            }
        }

        #endregion
    }
}
