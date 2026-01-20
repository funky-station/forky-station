// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 brainfood1183
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Interaction.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class UnremoveableComponent : Component
{
    /// <summary>
    /// If this is true then unremovable items that are removed from inventory are deleted (typically from corpse gibbing).
    /// Items within unremovable containers are not deleted when removed.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool DeleteOnDrop = true;
}
