// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 778b
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server.Spawners.Components;

[RegisterComponent]
public sealed partial class SpawnPointComponent : Component, ISpawnPoint
{
    /// <summary>
    /// The job this spawn point is valid for.
    /// Null will allow all jobs to spawn here.
    /// </summary>
    [DataField("job_id")]
    public ProtoId<JobPrototype>? Job;

    /// <summary>
    /// The type of spawn point.
    /// </summary>
    [DataField("spawn_type"), ViewVariables(VVAccess.ReadWrite)]
    public SpawnPointType SpawnType { get; set; } = SpawnPointType.Unset;

    public override string ToString()
    {
        return $"{Job} {SpawnType}";
    }
}

public enum SpawnPointType
{
    Unset = 0,
    LateJoin,
    Job,
    Observer,
}
