// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-License-Identifier: MIT

using Content.Shared.Anomaly;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Anomaly.Components;

[RegisterComponent, Access(typeof(SharedAnomalySystem)), AutoGenerateComponentPause]
public sealed partial class GeneratingAnomalyGeneratorComponent : Component
{
    /// <summary>
    /// When the generating period will end.
    /// </summary>
    [DataField("endTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan EndTime = TimeSpan.Zero;

    public EntityUid? AudioStream;
}
