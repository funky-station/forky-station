// SPDX-FileCopyrightText: 2025 Errant
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Mind;

namespace Content.Shared.Roles;

/// <summary>
///     Raised on mind entities when a mind role is removed from them.
/// </summary>
/// <param name="MindId">The mind id associated with the player.</param>
/// <param name="Mind">The mind component associated with the mind id.</param>
/// <param name="RoleTypeUpdate">True if this update has changed the mind's role type</param>
public sealed record RoleRemovedEvent(EntityUid MindId, MindComponent Mind, bool RoleTypeUpdate) : RoleEvent(MindId, Mind, RoleTypeUpdate);
