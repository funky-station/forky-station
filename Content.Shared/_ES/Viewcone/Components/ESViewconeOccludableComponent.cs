using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Physics;

namespace Content.Shared._ES.Viewcone.Components;

/// <summary>
///     Marks an entity as one which should fade away clientside if you have a viewcone and it's out of view
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ESViewconeOccludableComponent : Component, IComponentTreeEntry<ESViewconeOccludableComponent>
{
    [DataField, AutoNetworkedField]
    public bool OccludeIfAnchored = false;

    /// <summary>
    ///     Whether the occluding should be inverted,
    ///     i.e. the sprite will be invisible while within view, and visible outside of view
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Inverted = false;

    /// <summary>
    ///     If true, viewcone alpha handling will always override the base alpha of this entity when setting transparency.
    ///     Useful for viewcone effects.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool OverrideBaseAlpha = false;

    /// <summary>
    ///     If this is a temporary entity (like an effect), then this is the originating player (or other source)
    ///     of this occludable.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Source = null;

    /// <summary>
    ///     Kind of strange but essentially this is to help handle adding occludable dynamically to objects
    ///     that are being pulled, assuming they don't already have it. If they don't already have it, this is set
    ///     and this component will be removed when the object is no longer pulled.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool RemoveOnPullDropped = false;

    // Clientside comptree stuff
    public EntityUid? TreeUid { get; set; }
    public DynamicTree<ComponentTreeEntry<ESViewconeOccludableComponent>>? Tree { get; set; }
    public bool AddToTree => true;
    public bool TreeUpdateQueued { get; set; }
}
