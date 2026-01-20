// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.Roles;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Server.Station.Events;

[ByRefEvent]
public readonly record struct StationJobsGetCandidatesEvent(NetUserId Player, List<ProtoId<JobPrototype>> Jobs);
