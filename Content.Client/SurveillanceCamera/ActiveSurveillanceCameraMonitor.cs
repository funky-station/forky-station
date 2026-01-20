// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

namespace Content.Client.SurveillanceCamera;

[RegisterComponent]
public sealed partial class ActiveSurveillanceCameraMonitorVisualsComponent : Component
{
    public float TimeLeft = 10f;

    public Action? OnFinish;
}
