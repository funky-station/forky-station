// SPDX-FileCopyrightText: 2022 Moony <moony@hellomouse.net>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Moomoobeef <62638182+Moomoobeef@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Traits.Assorted;

/// <summary>
/// This is used for making something blind forever.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PermanentBlindnessComponent : Component
{
    /// <summary>
    /// How damaged should their eyes be? Set 0 for maximum damage.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Blindness = 0;
}

