using Content.Shared._Funkystation.SM.Components;
using Robust.Shared.Containers;

namespace Content.Server._Funkystation.SM.Events;

/// <summary>
/// Event raised on the entity being consumed whenever a supermatter consumes an entity.
/// </summary>
[ByRefEvent]
public readonly record struct SupermatterAshedEntityEvent(EntityUid entity, EntityUid supermatterUid, SharedSupermatterComponent supermatter, BaseContainer? container , bool fromTree, bool isMob)
{
    /// <summary>
    /// The entity being consumed by the supermatter.
    /// </summary>
    public readonly EntityUid Entity = entity;

    /// <summary>
    /// The uid of the supermatter consuming the entity.
    /// </summary>
    public readonly EntityUid SupermatterUid = supermatterUid;

    /// <summary>
    /// The supermatter consuming the target entity.
    /// </summary>
    public readonly SharedSupermatterComponent Supermatter = supermatter;

    /// <summary>
    /// The innermost container of the entity being consumed by the supermatter that is not also in the process of being consumed by the supermatter.
    /// Used to correctly dump out the contents containers that are consumed by the supermatter.
    /// </summary>
    public readonly BaseContainer? Container = container;

    public readonly bool FromContainerTree = fromTree;

    public readonly bool IsMob = isMob;
}
