// SPDX-FileCopyrightText: 2026 pathetic meowmeow
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2025 mtrs163
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 TemporalOroboros
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Krunklehorn
// SPDX-FileCopyrightText: 2025 Ilya246
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 ElectroJr
// SPDX-FileCopyrightText: 2024 MilenVolf
// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2024 Simon
// SPDX-FileCopyrightText: 2024 no
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 Varen
// SPDX-FileCopyrightText: 2023 Kevin Zheng
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2022 Radrark
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Logs;
using Content.Server.Buckle.Systems;
using Content.Server.Parallax;
using Content.Server.Procedural;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Events;
using Content.Server.Station.Systems;
using Content.Server.Stunnable;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Gibbing;
using Content.Shared.Light.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Salvage;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Throwing;
using JetBrains.Annotations;
using Robust.Server.GameObjects;
using Robust.Server.GameStates;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared.Maps;

namespace Content.Server.Shuttles.Systems;

[UsedImplicitly]
public sealed partial class ShuttleSystem : SharedShuttleSystem
{
    [Dependency] private readonly IAdminLogManager _logger = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly BiomeSystem _biomes = default!;
    [Dependency] private readonly GibbingSystem _gibbing = default!;
    [Dependency] private readonly BuckleSystem _buckle = default!;
    [Dependency] private readonly DamageableSystem _damageSys = default!;
    [Dependency] private readonly DockingSystem _dockSystem = default!;
    [Dependency] private readonly DungeonSystem _dungeon = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly MapLoaderSystem _loader = default!;
    [Dependency] private readonly MapSystem _mapSystem = default!;
    [Dependency] private readonly MetaDataSystem _metadata = default!;
    [Dependency] private readonly PvsOverrideSystem _pvs = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedSalvageSystem _salvage = default!;
    [Dependency] private readonly ShuttleConsoleSystem _console = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly StunSystem _stuns = default!;
    [Dependency] private readonly ThrowingSystem _throwing = default!;
    [Dependency] private readonly ThrusterSystem _thruster = default!;
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly TurfSystem _turf = default!;

    private EntityQuery<BuckleComponent> _buckleQuery;
    private EntityQuery<MapGridComponent> _gridQuery;
    private EntityQuery<PhysicsComponent> _physicsQuery;
    private EntityQuery<TransformComponent> _xformQuery;

    public override void Initialize()
    {
        base.Initialize();

        _buckleQuery = GetEntityQuery<BuckleComponent>();
        _gridQuery = GetEntityQuery<MapGridComponent>();
        _physicsQuery = GetEntityQuery<PhysicsComponent>();
        _xformQuery = GetEntityQuery<TransformComponent>();

        InitializeFTL();
        InitializeGridFills();
        InitializeIFF();
        InitializeImpact();

        SubscribeLocalEvent<ShuttleComponent, ComponentStartup>(OnShuttleStartup);
        SubscribeLocalEvent<ShuttleComponent, ComponentShutdown>(OnShuttleShutdown);
        SubscribeLocalEvent<ShuttleComponent, TileFrictionEvent>(OnTileFriction);
        SubscribeLocalEvent<ShuttleComponent, FTLStartedEvent>(OnFTLStarted);
        SubscribeLocalEvent<ShuttleComponent, FTLCompletedEvent>(OnFTLCompleted);

        SubscribeLocalEvent<GridInitializeEvent>(OnGridInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateHyperspace();
    }

    private void OnGridInit(GridInitializeEvent ev)
    {
        if (HasComp<MapComponent>(ev.EntityUid))
            return;

        EnsureComp<ShuttleComponent>(ev.EntityUid);

        // This and RoofComponent should be mutually exclusive, so ImplicitRoof should be removed if the grid has RoofComponent
        if (HasComp<RoofComponent>(ev.EntityUid))
            RemComp<ImplicitRoofComponent>(ev.EntityUid);
        else
            EnsureComp<ImplicitRoofComponent>(ev.EntityUid);
    }

    private void OnShuttleStartup(EntityUid uid, ShuttleComponent component, ComponentStartup args)
    {
        if (!HasComp<MapGridComponent>(uid))
        {
            return;
        }

        if (!TryComp(uid, out PhysicsComponent? physicsComponent))
        {
            return;
        }

        if (component.Enabled)
        {
            Enable(uid, component: physicsComponent, shuttle: component);
        }

        component.DampingModifier = component.BodyModifier;
    }

    public void Toggle(EntityUid uid, ShuttleComponent component)
    {
        if (!TryComp(uid, out PhysicsComponent? physicsComponent))
            return;

        component.Enabled = !component.Enabled;

        if (component.Enabled)
        {
            Enable(uid, component: physicsComponent, shuttle: component);
        }
        else
        {
            Disable(uid, component: physicsComponent);
        }
    }

    public void Enable(EntityUid uid, FixturesComponent? manager = null, PhysicsComponent? component = null, ShuttleComponent? shuttle = null)
    {
        if (!Resolve(uid, ref manager, ref component, ref shuttle, false))
            return;

        _physics.SetBodyType(uid, BodyType.Dynamic, manager: manager, body: component);
        _physics.SetBodyStatus(uid, component, BodyStatus.InAir);
        _physics.SetFixedRotation(uid, false, manager: manager, body: component);
    }

    public void Disable(EntityUid uid, FixturesComponent? manager = null, PhysicsComponent? component = null)
    {
        if (!Resolve(uid, ref manager, ref component, false))
            return;

        _physics.SetBodyType(uid, BodyType.Static, manager: manager, body: component);
        _physics.SetBodyStatus(uid, component, BodyStatus.OnGround);
        _physics.SetFixedRotation(uid, true, manager: manager, body: component);
    }

    private void OnShuttleShutdown(EntityUid uid, ShuttleComponent component, ComponentShutdown args)
    {
        // None of the below is necessary for any cleanup if we're just deleting.
        if (Comp<MetaDataComponent>(uid).EntityLifeStage >= EntityLifeStage.Terminating)
            return;

        Disable(uid);
    }

    private void OnTileFriction(Entity<ShuttleComponent> ent, ref TileFrictionEvent args)
    {
        args.Modifier *= ent.Comp.DampingModifier;
    }

    private void OnFTLStarted(Entity<ShuttleComponent> ent, ref FTLStartedEvent args)
    {
        ent.Comp.DampingModifier = 0f;
    }

    private void OnFTLCompleted(Entity<ShuttleComponent> ent, ref FTLCompletedEvent args)
    {
        ent.Comp.DampingModifier = ent.Comp.BodyModifier;
    }
}
