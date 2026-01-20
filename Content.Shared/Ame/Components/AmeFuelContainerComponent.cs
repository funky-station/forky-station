// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 LordCarve
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Ame.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AmeFuelContainerComponent : Component
{
    /// <summary>
    /// The amount of fuel in the container.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int FuelAmount = 500;

    /// <summary>
    /// The maximum fuel capacity of the container.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int FuelCapacity = 500;
}
