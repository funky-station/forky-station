// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Justin Trotter
// SPDX-License-Identifier: MIT

using Content.Server.Shuttles.Systems;

namespace Content.Server.Shuttles.Events;

/// <summary>
/// Raised when <see cref="ShuttleSystem.FasterThanLight"/> has completed FTL Travel.
/// </summary>
[ByRefEvent]
public readonly record struct FTLCompletedEvent(EntityUid Entity, EntityUid MapUid);
