// SPDX-FileCopyrightText: 2025 Travis Reid
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Jezithyr
// SPDX-License-Identifier: MIT

namespace Content.Shared.Movement.Pulling.Events;

/// <summary>
/// Event raised directed BOTH at the puller and pulled entity when a pull stops.
/// </summary>
public sealed class PullStoppedMessage(EntityUid pullerUid, EntityUid pulledUid) : PullMessage(pullerUid, pulledUid);
