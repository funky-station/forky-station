// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2025 Travis Reid <86178026+Travis-G-Reid@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Movement.Pulling.Events;

/// <summary>
/// Event raised directed BOTH at the puller and pulled entity when a pull stops.
/// </summary>
public sealed class PullStoppedMessage(EntityUid pullerUid, EntityUid pulledUid) : PullMessage(pullerUid, pulledUid);
