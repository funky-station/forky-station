// SPDX-FileCopyrightText: 2025 Linkbro
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

namespace Content.Client.Power.Visualizers;

[RegisterComponent]
public sealed partial class CableVisualizerComponent : Component
{
    [DataField]
    public string? StatePrefix;

    [DataField]
    public string? ExtraLayerPrefix;
}
