// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 avery
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Server.Shuttles.Systems;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Shuttles.Components;

[RegisterComponent, Access(typeof(ArrivalsSystem)), AutoGenerateComponentPause]
public sealed partial class ArrivalsShuttleComponent : Component
{
    [DataField("station")]
    public EntityUid Station;

    [DataField("nextTransfer", customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextTransfer;

    [DataField("nextArrivalsTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextArrivalsTime;

    /// <summary>
    ///     the first arrivals FTL originates from nullspace instead of the station
    /// </summary>
    [DataField("firstRun")]
    public bool FirstRun = true;

}
