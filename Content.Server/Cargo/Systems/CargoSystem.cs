// SPDX-FileCopyrightText: 2022-2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022-2023 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023-2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023-2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 eoineoineoin <github@eoinrul.es>
// SPDX-FileCopyrightText: 2023 Checkraze <71046427+Cheackraze@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Fildrance <fildrance@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 icekot8 <93311212+icekot8@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Jake Huxell <JakeHuxell@pm.me>
// SPDX-FileCopyrightText: 2025 Samuka-C <47865393+Samuka-C@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 pathetic meowmeow <uhhadd@gmail.com>
// SPDX-FileCopyrightText: 2025 Milon <milonpl.git@proton.me>
// SPDX-License-Identifier: MIT

using Content.Server.Cargo.Components;
using Content.Server.DeviceLinking.Systems;
using Content.Server.Popups;
using Content.Server.Stack;
using Content.Server.Station.Systems;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Server.Radio.EntitySystems;
using Content.Shared.Cargo;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Mobs.Components;
using Content.Shared.Paper;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Cargo.Systems;

public sealed partial class CargoSystem : SharedCargoSystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly AccessReaderSystem _accessReaderSystem = default!;
    [Dependency] private readonly DeviceLinkSystem _linker = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly ItemSlotsSystem _slots = default!;
    [Dependency] private readonly PaperSystem _paperSystem = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly PricingSystem _pricing = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly StackSystem _stack = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly MetaDataSystem _metaSystem = default!;
    [Dependency] private readonly RadioSystem _radio = default!;

    private EntityQuery<TransformComponent> _xformQuery;
    private EntityQuery<CargoSellBlacklistComponent> _blacklistQuery;
    private EntityQuery<MobStateComponent> _mobQuery;
    private EntityQuery<TradeStationComponent> _tradeQuery;

    private HashSet<EntityUid> _setEnts = new();
    private List<EntityUid> _listEnts = new();
    private List<(EntityUid, CargoPalletComponent, TransformComponent)> _pads = new();

    public override void Initialize()
    {
        base.Initialize();

        _xformQuery = GetEntityQuery<TransformComponent>();
        _blacklistQuery = GetEntityQuery<CargoSellBlacklistComponent>();
        _mobQuery = GetEntityQuery<MobStateComponent>();
        _tradeQuery = GetEntityQuery<TradeStationComponent>();

        InitializeConsole();
        InitializeShuttle();
        InitializeTelepad();
        InitializeBounty();
        InitializeFunds();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        UpdateConsole();
        UpdateTelepad(frameTime);
        UpdateBounty();
    }
}
