// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

using Content.Shared.SurveillanceCamera;

namespace Content.Client.SurveillanceCamera;

// Dummy component so that targetted events work on client for
// appearance events.
[RegisterComponent]
public sealed partial class SurveillanceCameraVisualsComponent : Component
{
    [DataField("sprites")]
    public Dictionary<SurveillanceCameraVisuals, string> CameraSprites = new();
}
