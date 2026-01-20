// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Visuals;

/// <summary>
/// Used for the visualizer
/// </summary>
[Serializable, NetSerializable]
public enum PortableScrubberVisuals : byte
{
    IsFull,
    IsRunning,
    IsDraining,
}
