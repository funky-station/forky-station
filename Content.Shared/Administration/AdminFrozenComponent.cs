// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2024 no
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Administration;

[RegisterComponent, Access(typeof(AdminFrozenSystem))]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AdminFrozenComponent : Component
{
    /// <summary>
    /// Whether the player is also muted.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Muted;
}
