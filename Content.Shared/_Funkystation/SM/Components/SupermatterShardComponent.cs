using Content.Shared.Stacks;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.SM.Components;

/// <summary>
/// A small shard of the supermatter, can be combined into a full supermatter crystal
/// </summary>
[NetworkedComponent, RegisterComponent]
public sealed partial class SupermatterShardComponent : Component
{
    [DataField("shardPower")]
    public float ShardPower = 1000f;
}
