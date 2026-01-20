// SPDX-FileCopyrightText: 2025 Pok
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Bed.Components;

/// <summary>
/// Tracking component added to entities buckled to stasis beds.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(BedSystem))]
public sealed partial class StasisBedBuckledComponent : Component;
