using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;



namespace Content.Shared._Funkystation.Traits.Assorted;

[RegisterComponent, NetworkedComponent]
public sealed partial class HayFeverComponent : Component
{

    /// <summary>
    /// Timer between rolls for allergic reactions.
    /// </summary>
    [DataField("ReactionInterval",required: true)]
    public float ReactionInterval = 45f;

    /// <summary>
    /// Timer from warning to sneeze.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float NextReactionTime;

    /// <summary>
    /// The amount of time in seconds until a sneeze happens when triggered.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField]
    public float SneezeDelay;

    /// <summary>
    /// Time (in seconds) since the last allergic reaction.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField]
    public float TimeSinceReaction;

    /// <summary>
    /// Current time (in seconds) since a sneeze or sneeze attack timer started.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField]
    public float NextSneezeTime;
    /// <summary>
    /// Determines which allergic reaction will happen.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), DataField]
    public int ReactionType;


    [ViewVariables(VVAccess.ReadOnly), DataField]
    public int SneezesQueued;
}
