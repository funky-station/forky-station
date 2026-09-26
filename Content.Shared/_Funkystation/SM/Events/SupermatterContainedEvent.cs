using Content.Shared._Funkystation.SM.Components;
using Robust.Shared.Containers;

namespace Content.Server._Funkystation.SM.Events;

/// <summary>
/// An event queued when a supermatter is contained (put into a container).
/// Exists to delay the supermatter eating its way out of the container until events relating to the insertion have been processed.
/// Needs to be a class because ref structs can't be put into the queue.
/// </summary>
public sealed class SupermatterContainedEvent : EntityEventArgs
{
    /// <summary>
    /// The uid of the supermatter that has been contained.
    /// </summary>
    public readonly EntityUid Entity;

    /// <summary>
    /// The state of the supermatter that has been contained.
    /// </summary>
    public readonly SharedSupermatterComponent Supermatter;

    /// <summary>
    /// The arguments of the action that resulted in the supermatter being contained.
    /// </summary>
    public readonly EntGotInsertedIntoContainerMessage Args;

    public SupermatterContainedEvent(EntityUid entity, SharedSupermatterComponent supermatter, EntGotInsertedIntoContainerMessage args)
    {
        Entity = entity;
        Supermatter = supermatter;
        Args = args;
    }
}
