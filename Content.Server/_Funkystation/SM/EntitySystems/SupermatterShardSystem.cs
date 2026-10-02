using Content.Shared._Funkystation.SM.Components;

namespace Content.Server._Funkystation.SM.EntitySystems;

/// <summary>
/// Shards are marked with <see cref="SupermatterShardComponent"/> so main-crystal systems (reproduction, anomalies)
/// can exclude them without relying on prototype IDs.
/// </summary>
public sealed class SupermatterShardSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
    }
}
