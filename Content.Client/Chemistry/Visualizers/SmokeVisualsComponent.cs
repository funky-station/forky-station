// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-License-Identifier: MIT

namespace Content.Client.Chemistry.Visualizers;

/// <summary>
/// A component that changes color to match its contained reagents.
/// Managed by <see cref="SmokeVisualizerSystem"/>.
/// Only functions with smoke at the moment.
/// </summary>
[RegisterComponent]
[Access(typeof(SmokeVisualizerSystem))]
public sealed partial class SmokeVisualsComponent : Component {}
