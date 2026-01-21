// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-License-Identifier: MIT

namespace Content.Server.Xenoarchaeology.Artifact.XAE.Components;

/// <summary>
/// Effect of EMP on activation.
/// </summary>
[RegisterComponent, Access(typeof(XAEEmpInAreaSystem))]
public sealed partial class XAEEmpInAreaComponent : Component
{
    /// <summary>
    /// Range of EMP effect.
    /// </summary>
    [DataField]
    public float Range = 4f;

    /// <summary>
    /// Energy to be consumed from energy containers.
    /// </summary>
    [DataField]
    public float EnergyConsumption = 1000000;

    /// <summary>
    /// Duration (in seconds) for which devices going to be disabled.
    /// </summary>
    [DataField]
    public TimeSpan DisableDuration = TimeSpan.FromSeconds(60);
}
