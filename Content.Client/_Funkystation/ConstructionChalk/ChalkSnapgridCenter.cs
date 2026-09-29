using Content.Shared.Interaction;
using Robust.Client.Placement;
using Robust.Client.Placement.Modes;
using Robust.Client.Player;
using Robust.Shared.Map;

namespace Content.Client._Funkystation.ConstructionChalk;

public sealed partial class ChalkSnapgridCenter : SnapgridCenter
{
    [Dependency] private IEntityManager _entityManager = null!;
    [Dependency] private IPlayerManager _playerManager = null!;

    private readonly SharedTransformSystem _transformSystem;

    public ChalkSnapgridCenter(PlacementManager pMan) : base(pMan)
    {
        IoCManager.InjectDependencies(this);
        _transformSystem = _entityManager.System<SharedTransformSystem>();
    }

    public override bool IsValidPosition(EntityCoordinates position)
    {
        // skip check entirely for editor placement
        if (pManager.CurrentPermission is not { UseEditorContext: false, Range: > 0 })
            return base.IsValidPosition(position);

        if (_playerManager.LocalSession?.AttachedEntity is not { } player ||
            !_entityManager.TryGetComponent<TransformComponent>(player, out var xform))
        {
            return false;
        }

        if (!_transformSystem.InRange(xform.Coordinates, position, SharedInteractionSystem.InteractionRange))
        {
            InvalidPlaceColor = InvalidPlaceColor.WithAlpha(0f);
            return false;
        }

        InvalidPlaceColor = InvalidPlaceColor.WithAlpha(1f);

        return base.IsValidPosition(position);
    }
}
