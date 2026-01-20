// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2025 Errant
// SPDX-FileCopyrightText: 2024 Repo
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2022 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Content.Shared.Mind;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable, NetSerializable]
public sealed record PlayerInfo(
    string Username,
    string CharacterName,
    string IdentityName,
    string StartingJob,
    bool Antag,
    ProtoId<RoleTypePrototype>? RoleProto,
    LocId? Subtype,
    int SortWeight,
    NetEntity? NetEntity,
    NetUserId SessionId,
    bool Connected,
    bool ActiveThisRound,
    TimeSpan? OverallPlaytime)
{
    private string? _playtimeString;

    public bool IsPinned { get; set; }

    public string PlaytimeString => _playtimeString ??=
        OverallPlaytime?.ToString("%d':'hh':'mm") ?? Loc.GetString("generic-unknown-title");

    public bool Equals(PlayerInfo? other)
    {
        return other?.SessionId == SessionId;
    }

    public override int GetHashCode()
    {
        return SessionId.GetHashCode();
    }
}
