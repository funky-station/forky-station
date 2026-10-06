using Content.Shared._FarHorizons.Power.Generation.FissionGenerator;
using Robust.Client.GameObjects;

namespace Content.Client._FarHorizons.Power.Generation.FissionGenerator;

public sealed partial class ReactorPartSystem : SharedReactorPartSystem
{
    [Dependency] private SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ReactorPartComponent, AppearanceChangeEvent>(OnAppearanceChange);
        SubscribeLocalEvent<ReactorPartComponent, ComponentInit>(OnComponentInit);
    }

    private void OnAppearanceChange(EntityUid uid, ReactorPartComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        // Re-enable if/when there are multiple sprites
        //if (!_sprite.LayerMapTryGet((uid, args.Sprite), ReactorCapVisualLayers.Sprite, out var layer, false))
        //    return;

        _sprite.LayerSetColor((uid, args.Sprite), 0, ProtoMan.Index(component.Material).Color);
    }

    private void OnComponentInit(Entity<ReactorPartComponent> ent, ref ComponentInit args)
    {
        _sprite.LayerSetColor((ent.Owner, Comp<SpriteComponent>(ent.Owner)), 0, ProtoMan.Index(ent.Comp.Material).Color);
    }
}
