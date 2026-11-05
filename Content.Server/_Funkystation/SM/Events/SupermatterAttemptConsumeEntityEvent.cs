using Content.Shared._Funkystation.SM.Components;
using Robust.Shared.Containers;

namespace Content.Server._Funkystation.SM.Events;

[ByRefEvent]
public record struct SupermatterAttemptConsumeEntityEvent(EntityUid entity, EntityUid supermatterUid, SharedSupermatterComponent supermatter)
{
    /// <summary>
    /// The entity that the supermatter is attempting to consume.
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
    /// Whether the supermatter has been prevented from consuming the target entity.
    /// </summary>
    public bool Cancelled = false;
};
