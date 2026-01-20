// SPDX-FileCopyrightText: 2024 TakoDragon
// SPDX-FileCopyrightText: 2024 brainfood1183
// SPDX-License-Identifier: MIT

using Content.Server.Speech.EntitySystems;

namespace Content.Server.Speech.Components;

/// <summary>
/// French accent replaces spoken letters. "th" becomes "z" and "H" at the start of a word becomes "'".
/// </summary>
[RegisterComponent]
[Access(typeof(FrenchAccentSystem))]
public sealed partial class FrenchAccentComponent : Component {}
