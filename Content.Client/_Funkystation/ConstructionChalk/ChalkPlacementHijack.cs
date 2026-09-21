using System.Linq;
using Content.Client.Construction;
using Content.Client._Funkystation.Placement;
using Content.Shared.Construction.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Placement;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client._Funkystation.ConstructionChalk;

public sealed class ChalkPlacementHijack(
    IEntityManager entMan,
    IPrototypeManager protoMan,
    EntityUid chalk,
    ConstructionPrototype prototype,
    Action<EntityUid, string, EntityCoordinates, Angle> onPlace)
    : PlacementHijack, IAtmosPipeLayerHijack
{
    private readonly ConstructionSystem _constructionSystem = entMan.System<ConstructionSystem>();
    private readonly SpriteSystem _spriteSystem = entMan.System<SpriteSystem>();

    public override bool CanRotate { get; } = prototype.CanRotate;

    public ConstructionPrototype CurrentPrototype => prototype;

    public EntityUid Chalk => chalk;

    public PlacementHijack WithPrototype(ConstructionPrototype newPrototype)
    {
        return new ChalkPlacementHijack(entMan, protoMan, chalk, newPrototype, onPlace);
    }

    public override bool HijackPlacementRequest(EntityCoordinates coordinates)
    {
        onPlace(chalk, prototype.ID, coordinates, Manager.Direction.ToAngle());
        return true;
    }

    public override bool HijackDeletion(EntityUid entity)
    {
        return true;
    }

    public override void StartHijack(PlacementManager manager)
    {
        base.StartHijack(manager);

        if (!_constructionSystem.TryGetRecipePrototype(prototype.ID, out var targetProtoId))
            return;

        if (!protoMan.TryIndex(targetProtoId, out var proto))
            return;

        manager.CurrentTextures = _spriteSystem.GetPrototypeTextures(proto).ToList();
    }
}
