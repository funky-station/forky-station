using System.Linq;
using Content.Client.Construction;
using Content.Client.UserInterface.Controls;
using Content.Shared._Funkystation.ConstructionChalk;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Interaction.Events;
using Robust.Client.GameObjects;
using Robust.Client.Placement;
using Robust.Shared.Enums;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._Funkystation.ConstructionChalk;

public sealed partial class ConstructionChalkSystem : SharedConstructionChalkSystem
{
    [Dependency] private IEntityManager _entMan = null!;
    [Dependency] private IPrototypeManager _proto = null!;
    [Dependency] private IPlacementManager _placementManager = null!;
    [Dependency] private ConstructionSystem _construction = null!;
    [Dependency] private SpriteSystem _sprite = null!;
    [Dependency] private IGameTiming _timing = null!;

    private SimpleRadialMenu? _menu;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ConstructionChalkComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<ConstructionChalkMarkComponent, AfterAutoHandleStateEvent>(OnMarkState);
        SubscribeLocalEvent<ConstructionChalkComponent, EntParentChangedMessage>(OnParentChanged);
    }

    private void OnParentChanged(Entity<ConstructionChalkComponent> ent, ref EntParentChangedMessage args)
    {
        if ((_placementManager as PlacementManager)?.Hijack is ChalkPlacementHijack hijack &&
        hijack.Chalk == ent.Owner)
        {
            _placementManager.Clear();
        }
    }

    private void OnUseInHand(Entity<ConstructionChalkComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        if (!_timing.IsFirstTimePredicted)
            return;

        OpenMenu(ent);
    }

    private void OpenMenu(Entity<ConstructionChalkComponent> chalk)
    {
        _menu?.Close();
        _menu = new SimpleRadialMenu();

        var options = new List<RadialMenuOptionBase>();

        foreach (var category in _proto.EnumeratePrototypes<ChalkCategoryPrototype>())
        {
            if (category.Mode != chalk.Comp.Mode)
                continue;

            var entries = new List<RadialMenuOptionBase>();

            foreach (var entry in category.Entries)
            {
                var protoId = entry.ConstructionPrototype.Id;

                entries.Add(new RadialMenuActionOption<string>(
                    s => OnEntrySelected(chalk, s),
                    protoId)
                {
                    ToolTip = Loc.GetString(entry.Name),
                    IconSpecifier = RadialMenuIconSpecifier.With(entry.Icon),
                });
            }

            options.Add(new RadialMenuNestedLayerOption(entries)
            {
                ToolTip = Loc.GetString(category.Name),
                IconSpecifier = RadialMenuIconSpecifier.With(category.Icon),
            });
        }

        _menu.SetButtons(options);
        _menu.OpenOverMouseScreenPosition();
    }

    private void OnEntrySelected(Entity<ConstructionChalkComponent> chalk, string constructionPrototype)
    {
        if (!_proto.TryIndex<ConstructionPrototype>(constructionPrototype, out var recipe))
            return;

        var hijack = new ChalkPlacementHijack(_entMan, _proto, chalk.Owner, recipe, RequestPlaceMark);

        _placementManager.BeginPlacing(new PlacementInformation
        {
            IsTile = false,
            PlacementOption = recipe.PlacementMode,
        },
        hijack);
    }

    private void RequestPlaceMark(EntityUid chalk, string constructionPrototype, EntityCoordinates coordinates, Angle rotation)
    {
        RaisePredictiveEvent(new ChalkPlaceMarkEvent(
            GetNetEntity(chalk),
            GetNetCoordinates(coordinates),
            constructionPrototype,
            rotation));
    }

    // generic ghost visuals for now
    private void OnMarkState(Entity<ConstructionChalkMarkComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (ent.Comp.ConstructionPrototype is not { } recipeId ||
            !_construction.TryGetRecipePrototype(recipeId.Id, out var targetProtoId) ||
            !_proto.TryIndex(targetProtoId, out var targetProto))
        {
            return;
        }

        if (!TryComp<SpriteComponent>(ent.Owner, out var sprite))
            return;

        if (targetProto.TryComp(out IconComponent? icon, EntityManager.ComponentFactory))
        {
            _sprite.AddBlankLayer((ent.Owner, sprite), 0);
            _sprite.LayerSetSprite((ent.Owner, sprite), 0, icon.Icon);
            sprite.LayerSetShader(0, "unshaded");
            _sprite.LayerSetVisible((ent.Owner, sprite), 0, true);
        }
        else if (targetProto.Components.ContainsKey("Sprite"))
        {
            var dummy = _entMan.SpawnEntity(targetProtoId, MapCoordinates.Nullspace);
            var targetSprite = EnsureComp<SpriteComponent>(dummy);
            _entMan.System<AppearanceSystem>().OnChangeData(dummy, targetSprite);
            var markDrawDepth = sprite.DrawDepth;
            _sprite.CopySprite((dummy, targetSprite), (ent.Owner, sprite));
            _sprite.SetDrawDepth((ent.Owner, sprite), markDrawDepth);

            for (var i = 0; i < sprite.AllLayers.Count(); i++)
            {
                sprite.LayerSetShader(i, "unshaded");
            }

            Del(dummy);
        }
        else
        {
            return;
        }

        _sprite.SetColor((ent.Owner, sprite), new Color(200, 200, 200, 140));
    }
}
