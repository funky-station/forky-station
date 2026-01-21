// SPDX-FileCopyrightText: 2024-2025 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Holopad;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HolographicAvatarComponent : Component
{
    /// <summary>
    /// The prototype sprite layer data for the hologram
    /// </summary>
    [DataField, AutoNetworkedField]
    public PrototypeLayerData[]? LayerData = null;
}
