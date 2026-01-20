// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-License-Identifier: MIT

namespace Content.Shared.Movement.Pulling.Events;

/// <summary>
/// Event raised directed BOTH at the puller and pulled entity when a pull starts.
/// </summary>
public sealed class PullStartedMessage(EntityUid pullerUid, EntityUid pullableUid) : PullMessage(pullerUid, pullableUid);
