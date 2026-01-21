// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Linkbro <104574466+Linkbro1@users.noreply.github.com>
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
