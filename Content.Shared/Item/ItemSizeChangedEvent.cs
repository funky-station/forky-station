// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Item;

/// <summary>
/// Raised directed on an entity when its item size / shape changes.
/// </summary>
[ByRefEvent]
public struct ItemSizeChangedEvent(EntityUid Entity)
{
    public EntityUid Entity = Entity;
}
