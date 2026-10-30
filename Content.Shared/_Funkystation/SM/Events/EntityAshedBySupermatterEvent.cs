using Content.Shared._Funkystation.SM.Components;
using Robust.Shared.Containers;

namespace Content.Server._Funkystation.SM.Events;
/// <summary>
///     Event raised on the supermatter entity whenever a supermatter ash an entity.
/// </summary>
[ByRefEvent]
public readonly record struct EntityAshedBySupermatterEvent (EntityUid entity, EntityUid supermatterUid, SharedSupermatterComponent supermatter, BaseContainer? container, bool fromTree)
{
    /// <summary>
    /// The entity being ashed by the supermatter.
    /// </summary>
    public readonly EntityUid Entity = entity;

    /// <summary>
    /// The uid of the supermatter ashing the entity.
    /// </summary>
    public readonly EntityUid SupermatterUid = supermatterUid;

    /// <summary>
    /// The supermatter ashing the target entity.
    /// </summary>
    public readonly SharedSupermatterComponent Supermatter = supermatter;

    /// <summary>
    /// The innermost container of the entity being ashed by the supermatter that is not also in the process of being ashed by the supermatter.
    /// Used to correctly dump out the contents containers that are ashed by the supermatter.
    /// </summary>
    public readonly BaseContainer? Container = container;

    public readonly bool FromContainerTree = fromTree;


}
