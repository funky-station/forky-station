using Content.Shared._Funkystation.SM.Components;
using Content.Shared.Stacks;

namespace Content.Shared._Funkystation.SM;

/// <summary>
/// Shards are marked with <see cref="SupermatterShardComponent"/> so main-crystal systems (reproduction, anomalies)
/// can exclude them without relying on prototype IDs.
/// </summary>
public sealed partial class SupermatterShardSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SupermatterShardComponent, StackCountChangedEvent>(OnStackChanged);

    }

    private void OnStackChanged(EntityUid uid, SupermatterShardComponent shard, StackCountChangedEvent args)
    {
        Logger.Info($"new count {args.NewCount}, old count {args.OldCount}");
        if (args.NewCount < 100)
            return;
        Logger.Info($"100? {args.NewCount}, old count {args.OldCount}");
        var coords = Transform(uid).Coordinates;

        SpawnAtPosition("Supermatter", coords);

        QueueDel(uid);

    }
}
