// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 TinManTim
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

namespace Content.Shared.Destructible;

public abstract class SharedDestructibleSystem : EntitySystem
{
    /// <summary>
    /// Force entity to be destroyed and deleted.
    /// </summary>
    public bool DestroyEntity(Entity<MetaDataComponent?> owner)
    {
        var ev = new DestructionAttemptEvent();
        RaiseLocalEvent(owner, ev);
        if (ev.Cancelled)
            return false;

        var eventArgs = new DestructionEventArgs();
        RaiseLocalEvent(owner, eventArgs);

        PredictedQueueDel(owner);
        return true;
    }

    /// <summary>
    /// Force entity to break.
    /// </summary>
    public void BreakEntity(EntityUid owner)
    {
        var eventArgs = new BreakageEventArgs();
        RaiseLocalEvent(owner, eventArgs);
    }
}

/// <summary>
/// Raised before an entity is about to be destroyed and deleted
/// </summary>
public sealed class DestructionAttemptEvent : CancellableEntityEventArgs
{

}

/// <summary>
/// Raised when entity is destroyed and about to be deleted.
/// </summary>
public sealed class DestructionEventArgs : EntityEventArgs
{

}

/// <summary>
/// Raised when entity was heavy damage and about to break.
/// </summary>
public sealed class BreakageEventArgs : EntityEventArgs
{

}
