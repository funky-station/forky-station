// SPDX-FileCopyrightText: 2023 Morb
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.NPC;

/// <summary>
/// Added to NPCs that are actively being updated.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveNPCComponent : Component {}
