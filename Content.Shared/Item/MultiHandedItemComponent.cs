// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Item;

/// <summary>
/// This is used for items that need
/// multiple hands to be able to be picked up
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class MultiHandedItemComponent : Component
{
    [DataField]
    public int HandsNeeded = 2;
}
