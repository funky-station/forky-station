// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 Tao
// SPDX-FileCopyrightText: 2025 keronshb
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 lzk
// SPDX-FileCopyrightText: 2024 MilenVolf
// SPDX-FileCopyrightText: 2023 Repo
// SPDX-FileCopyrightText: 2022 Ian Pike
// SPDX-FileCopyrightText: 2022 Jacob Tong
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Daniel Castro Razo
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2020 ancientpower
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2020 ComicIronic
// SPDX-FileCopyrightText: 2018 Acruid
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2019 L.E.D
// SPDX-FileCopyrightText: 2019 PrPleGoo
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Serialization;

namespace Content.Shared.Physics;

/// <summary>
///     Defined collision groups for the physics system.
///     Mask is what it collides with when moving. Layer is what CollisionGroup it is part of.
/// </summary>
[Flags, PublicAPI]
[FlagsFor(typeof(CollisionLayer)), FlagsFor(typeof(CollisionMask))]
public enum CollisionGroup
{
    None               = 0,
    Opaque             = 1 << 0, // 1 Blocks light, can be hit by lasers
    Impassable         = 1 << 1, // 2 Walls, objects impassable by any means
    MidImpassable      = 1 << 2, // 4 Mobs, players, crabs, etc
    HighImpassable     = 1 << 3, // 8 Things on top of tables and things that block tall/large mobs.
    LowImpassable      = 1 << 4, // 16 For things that can fit under a table or squeeze under an airlock
    GhostImpassable    = 1 << 5, // 32 Things impassible by ghosts/observers, ie blessed tiles or forcefields
    BulletImpassable   = 1 << 6, // 64 Can be hit by bullets
    InteractImpassable = 1 << 7, // 128 Blocks interaction/InRangeUnobstructed
    // Y dis door passable when all the others impassable / collision.
    DoorPassable       = 1 << 8, // 256 Allows door to close over top, Like blast doors over conveyors for disposals rooms/cargo.

    MapGrid = MapGridHelpers.CollisionGroup, // Map grids, like shuttles. This is the actual grid itself, not the walls or other entities connected to the grid.

    // 32 possible groups
    // Why dis exist
    AllMask = -1,

    SingularityLayer = Opaque | Impassable | MidImpassable | HighImpassable | LowImpassable | BulletImpassable | InteractImpassable | DoorPassable,

    // Humanoids, etc.
    MobMask = Impassable | HighImpassable | MidImpassable | LowImpassable,
    MobLayer = Opaque | BulletImpassable,
    // Mice, drones
    SmallMobMask = Impassable | LowImpassable,
    SmallMobLayer = Opaque | BulletImpassable,
    // Birds/other small flyers
    FlyingMobMask = Impassable | HighImpassable,
    FlyingMobLayer = Opaque | BulletImpassable,

    // Mechs
    LargeMobMask = Impassable | HighImpassable | MidImpassable | LowImpassable,
    LargeMobLayer = Opaque | HighImpassable | MidImpassable | LowImpassable | BulletImpassable,

    // Machines, computers
    MachineMask = Impassable | MidImpassable | LowImpassable,
    MachineLayer = Opaque | MidImpassable | LowImpassable | BulletImpassable,
    ConveyorMask = Impassable | MidImpassable | LowImpassable | DoorPassable,

    // Crates
    CrateMask = Impassable | HighImpassable | LowImpassable,

    // Tables that SmallMobs can go under
    TableMask = Impassable | MidImpassable,
    TableLayer = MidImpassable,

    // Tabletop machines, windoors, firelocks
    TabletopMachineMask = Impassable | HighImpassable,
    // Tabletop machines
    TabletopMachineLayer = Opaque | BulletImpassable,

    // Airlocks, windoors, firelocks
    GlassAirlockLayer = HighImpassable | MidImpassable | BulletImpassable | InteractImpassable,
    AirlockLayer = Opaque | GlassAirlockLayer,

    // Airlock assembly
    HumanoidBlockLayer = HighImpassable | MidImpassable,

    // Soap, spills
    SlipLayer = MidImpassable | LowImpassable,
    ItemMask = Impassable | HighImpassable,
    ThrownItem = Impassable | HighImpassable | BulletImpassable,
    WallLayer = Opaque | Impassable | HighImpassable | MidImpassable | LowImpassable | BulletImpassable | InteractImpassable,
    GlassLayer = Impassable | HighImpassable | MidImpassable | LowImpassable | BulletImpassable | InteractImpassable,
    HalfWallLayer = MidImpassable | LowImpassable,
    FlimsyLayer = Opaque | HighImpassable | MidImpassable | LowImpassable | InteractImpassable,

    // Allows people to interact past and target players inside of this
    SpecialWallLayer = Opaque | HighImpassable | MidImpassable | LowImpassable | BulletImpassable,

    // Statue, monument, airlock, window
    FullTileMask = Impassable | HighImpassable | MidImpassable | LowImpassable | InteractImpassable,
    // FlyingMob can go past
    FullTileLayer = Opaque | HighImpassable | MidImpassable | LowImpassable | BulletImpassable | InteractImpassable,

    SubfloorMask = Impassable | LowImpassable
}
