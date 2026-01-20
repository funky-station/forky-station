// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Morb
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Traits.Assorted;

/// <summary>
/// Set player speed to zero and standing state to down, simulating leg paralysis.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(LegsParalyzedSystem))]
public sealed partial class LegsParalyzedComponent : Component
{
}
