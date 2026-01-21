// SPDX-FileCopyrightText: 2025 mtrs163 <153596133+mtrs163@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Light.Components;

/// <summary>
/// Assumes the entire attached grid is rooved. This component will get removed if the grid has RoofComponent.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ImplicitRoofComponent : Component
{
    [DataField, AutoNetworkedField]
    public Color Color = Color.Black;
}
