// SPDX-FileCopyrightText: 2025 Pok <113675512+Pok27@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Bed.Components;

/// <summary>
/// Tracking component added to entities buckled to stasis beds.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(BedSystem))]
public sealed partial class StasisBedBuckledComponent : Component;
