// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Absotively
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2024 Krunklehorn
// SPDX-FileCopyrightText: 2025 Krunklehorn
// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-FileCopyrightText: 2025 Milon
// SPDX-FileCopyrightText: 2025 eoineoineoin
// SPDX-FileCopyrightText: 2025 Errant
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Managers;
using Content.Server.DeviceNetwork.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Emag.Systems;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Power.EntitySystems;
using Content.Shared.PowerCell;
using Content.Shared.Roles;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Trigger.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server.Silicons.Borgs;

/// <inheritdoc/>
public sealed partial class BorgSystem : SharedBorgSystem
{
    [Dependency] private readonly IBanManager _banManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly DeviceNetworkSystem _deviceNetwork = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedBatterySystem _battery = default!;
    [Dependency] private readonly EmagSystem _emag = default!;
    [Dependency] private readonly MobThresholdSystem _mobThresholdSystem = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly PowerCellSystem _powerCell = default!;

    public static readonly ProtoId<JobPrototype> BorgJobId = "Borg";

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        InitializeTransponder();
    }

    public override bool CanPlayerBeBorged(ICommonSession session)
    {
        if (_banManager.GetJobBans(session.UserId)?.Contains(BorgJobId) == true)
            return false;

        return true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        UpdateTransponder(frameTime);
    }
}
