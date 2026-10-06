using Content.Server.Atmos.EntitySystems;
using Content.Server.Destructible;
using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Shared.Atmos.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.Molotov;

// spawns a burning rag if the molotov was lit, a plain rag otherwise
[DataDefinition]
public sealed partial class SpawnBurningRagBehavior : IThresholdBehavior
{
    [DataField]
    public EntProtoId LitRag = "MolotovRag";

    [DataField]
    public EntProtoId UnlitRag = "RagItem";

    public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
    {
        var entMan = system.EntityManager;
        var coords = entMan.GetComponent<TransformComponent>(owner).Coordinates;
        var onFire = entMan.TryGetComponent<FlammableComponent>(owner, out var flammable) && flammable.OnFire;

        var ragProto = onFire ? LitRag : UnlitRag;
        var rag = entMan.SpawnEntity(ragProto, coords);

        if (onFire)
        {
            entMan.System<FlammableSystem>().Ignite(rag, rag);
        }
    }
}
