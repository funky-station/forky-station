// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Spreader;

[RegisterComponent, Access(typeof(KudzuSystem)), AutoGenerateComponentPause]
public sealed partial class GrowingKudzuComponent : Component
{
    /// <summary>
    /// The next time kudzu will try to tick its growth level.
    /// </summary>
    [DataField("nextTick", customTypeSerializer:typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextTick = TimeSpan.Zero;
}
