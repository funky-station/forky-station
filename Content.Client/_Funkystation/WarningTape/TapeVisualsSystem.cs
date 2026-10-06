using System.Numerics;
using Content.Client.Clickable;
using Content.Shared._Funkystation.WarningTape.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Prototypes;

namespace Content.Client._Funkystation.WarningTape;

public sealed partial class TapeVisualsSystem : EntitySystem
{
    [Dependency] private IOverlayManager _overlay = null!;
    [Dependency] private IPrototypeManager _prototype = null!;
    [Dependency] private SpriteSystem _sprite = null!;

    public override void Initialize()
    {
        base.Initialize();
        _overlay.AddOverlay(new TapeVisualsOverlay(EntityManager, _prototype));

        SubscribeLocalEvent<TapeVisualsComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<TapeVisualsComponent, AfterAutoHandleStateEvent>(OnVisualsUpdated);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlay.RemoveOverlay<TapeVisualsOverlay>();
    }

    private void OnStartup(Entity<TapeVisualsComponent> ent, ref ComponentStartup args)
    {
        UpdateClickableBounds(ent);
    }

    private void OnVisualsUpdated(Entity<TapeVisualsComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        UpdateClickableBounds(ent);
    }

    private void UpdateClickableBounds(Entity<TapeVisualsComponent> ent)
    {
        var tileLen = ent.Comp.ContentRegion.Width / EyeManager.PixelsPerMeter;
        var totalLength = ent.Comp.TileCount * tileLen;

        if (!TryComp<SpriteComponent>(ent, out var sprite))
            return;

        _sprite.SetOffset((ent.Owner, sprite), new Vector2(totalLength / 2f, 0f));
        _sprite.SetScale((ent.Owner, sprite), new Vector2(totalLength, 1f));
    }
}
