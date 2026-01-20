// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Sailor
// SPDX-FileCopyrightText: 2023 Vasilis
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.Power.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.PowerCell.Components;

/// <summary>
/// This component enables power-cell related interactions (e.g. EntityWhitelists, cell sizes, examine, rigging).
/// The actual power functionality is provided by the <see cref="BatteryComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PowerCellComponent : Component;
