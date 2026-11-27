using Content.Shared._AS.CanvasDesign;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;

namespace Content.Client._AS.CanvasDesign;

/// <summary>
/// Client side rendering and draft storage for <see cref="CanvasDesignComponent"/>.
/// Creates a texture from the component's pixel array and applies it to a sprite layer.
/// </summary>
public sealed partial class CanvasDesignSystem : EntitySystem
{
    /// <summary>
    /// The sprite layer key used to render the canvas texture. This layer is created automatically if it does not exist.
    /// </summary>
    public const string CanvasLayerKey = "canvas-design";

    [Dependency] private IClyde _clyde = null!;
    [Dependency] private SpriteSystem _sprite = null!;
    [Dependency] private AppearanceSystem _appearance = null!;

    private readonly Dictionary<EntityUid, CanvasDesignDraft> _drafts = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<CanvasDesignComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<CanvasDesignComponent, AfterAutoHandleStateEvent>(OnState);
        SubscribeLocalEvent<CanvasDesignComponent, AppearanceChangeEvent>(OnAppearanceChange);
        SubscribeLocalEvent<CanvasDesignComponent, CanvasDesignLocalPreviewEvent>(OnLocalPreview);
        SubscribeLocalEvent<CanvasDesignComponent, CanvasDesignLocalPreviewEndedEvent>(OnLocalPreviewEnded);
        SubscribeLocalEvent<CanvasDesignScreenComponent, ComponentShutdown>(OnScreenShutdown);
        SubscribeLocalEvent<CanvasDesignComponent, EntityTerminatingEvent>(OnTerminating);
    }

    private void OnStartup(Entity<CanvasDesignComponent> ent, ref ComponentStartup args)
    {
        var screen = EnsureComp<CanvasDesignScreenComponent>(ent);
        if (!EnsureTexture(ent, screen))
            return;

        Redraw(ent, screen);
        UpdateVisibility(ent);
    }

    private void OnState(Entity<CanvasDesignComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        var screen = EnsureComp<CanvasDesignScreenComponent>(ent);
        if (!EnsureTexture(ent, screen))
            return;

        UpdateShader(ent);
        Redraw(ent, screen);
        UpdateVisibility(ent);
    }

    private void OnAppearanceChange(Entity<CanvasDesignComponent> ent, ref AppearanceChangeEvent args)
    {
        UpdateVisibility(ent, args.Sprite, args.Component);
    }

    private void UpdateVisibility(Entity<CanvasDesignComponent> ent,
        SpriteComponent? sprite = null,
        AppearanceComponent? appearance = null)
    {
        if (!Resolve(ent.Owner, ref sprite, false) ||
            !_sprite.LayerMapTryGet((ent.Owner, sprite), CanvasLayerKey, out _, false))
            return;

        var visible = true;
        if (ent.Comp.RequirePowerToDisplay)
        {
            _appearance.TryGetData<bool>(ent.Owner, PowerDeviceVisuals.Powered, out var powered, appearance);
            visible = powered;
        }

        _sprite.LayerSetVisible((ent.Owner, sprite), CanvasLayerKey, visible);
    }

    private void OnLocalPreview(Entity<CanvasDesignComponent> ent, ref CanvasDesignLocalPreviewEvent args)
    {
        // The preview event is sent by the client to itself, so we don't need to check for authority here.
        // The preview should only be applied to the local client, and not sent to the server.
        if (TryComp<CanvasDesignScreenComponent>(ent, out var screen))
            Redraw(ent, screen, args.Pixels);
    }

    private void OnLocalPreviewEnded(Entity<CanvasDesignComponent> ent, ref CanvasDesignLocalPreviewEndedEvent args)
    {
        if (TryComp<CanvasDesignScreenComponent>(ent, out var screen))
            Redraw(ent, screen);
    }

    // Ensures that the canvas texture exists and is up to date with the component's pixel array. If the texture does not exist or is the wrong size, it will be created or resized.
    private bool EnsureTexture(Entity<CanvasDesignComponent> ent, CanvasDesignScreenComponent screen)
    {
        if (!TryComp<SpriteComponent>(ent, out var sprite))
            return false;

        if (!_sprite.LayerMapTryGet((ent.Owner, sprite), CanvasLayerKey, out _, false))
        {
            // Create a new layer for the canvas texture if it doesn't exist.
            _sprite.AddBlankLayer((ent.Owner, sprite));
            _sprite.LayerMapSet((ent.Owner, sprite), CanvasLayerKey, sprite.AllLayers.Count() - 1);
        }

        var rsiSize = sprite.BaseRSI?.Size;
        if (rsiSize == null)
            return false;

        var textureWidth = rsiSize.Value.X;
        var textureHeight = rsiSize.Value.Y;
        if (!ent.Comp.RenderBoundsAreValid(textureWidth, textureHeight))
            return false;

        if (screen.Texture != null && screen.TextureWidth == textureWidth && screen.TextureHeight == textureHeight)
            return true;

        screen.Texture?.Dispose();
        screen.TextureWidth = textureWidth;
        screen.TextureHeight = textureHeight;
        screen.Buffer = new Rgba32[textureWidth * textureHeight];
        screen.Texture = _clyde.CreateBlankTexture<Rgba32>((textureWidth, textureHeight), $"canvas-design-{ent.Owner}");
        _sprite.LayerSetTexture((ent.Owner, sprite), CanvasLayerKey, screen.Texture);
        UpdateShader(ent);
        return true;
    }

    // If the component has a shader specified, apply it to the canvas sprite layer. If the shader is null or empty, remove any existing shader from the layer.
    private void UpdateShader(Entity<CanvasDesignComponent> ent)
    {
        if (!TryComp<SpriteComponent>(ent, out var sprite) ||
            !_sprite.LayerMapTryGet((ent.Owner, sprite), CanvasLayerKey, out var layer, false))
            return;

        if (!string.IsNullOrWhiteSpace(ent.Comp.Shader))
            sprite.LayerSetShader(layer, ent.Comp.Shader);
        else
            sprite.LayerSetShader(layer, null, null);
    }

    /// <summary>
    /// Redraws the canvas texture from the component's pixel array or a provided preview array.
    /// </summary>
    private void Redraw(Entity<CanvasDesignComponent> ent, CanvasDesignScreenComponent screen, uint[]? preview = null)
    {
        var pixels = preview ?? ent.Comp.Pixels;
        if (screen.Texture == null || pixels.Length != ent.Comp.PixelCount ||
            !ent.Comp.RenderBoundsAreValid(screen.TextureWidth, screen.TextureHeight))
            return;

        Array.Fill(screen.Buffer, new Rgba32(0, 0, 0, 0));
        for (var y = 0; y < ent.Comp.Height; y++)
        for (var x = 0; x < ent.Comp.Width; x++)
        {
            var value = pixels[y * ent.Comp.Width + x];
            screen.Buffer[(y + ent.Comp.OffsetY) * screen.TextureWidth + x + ent.Comp.OffsetX] = new Rgba32(
                (byte) (value >> 16),
                (byte) (value >> 8),
                (byte) value,
                (byte) (value >> 24));
        }

        screen.Texture.SetSubImage((0, 0), (screen.TextureWidth, screen.TextureHeight), screen.Buffer);
    }

    private void OnScreenShutdown(Entity<CanvasDesignScreenComponent> ent, ref ComponentShutdown args)
    {
        ent.Comp.Texture?.Dispose();
        ent.Comp.Texture = null;
        ent.Comp.Buffer = [];
    }

    /// <summary>
    /// Stores an unsaved client-side draft associated with an entity. This draft is not sent to the server and will be discarded if the client disconnects or the entity is deleted.
    /// </summary>
    public void SetDraft(EntityUid uid, uint[] pixels, string name, string description)
    {
        if (pixels.Length > CanvasDesignComponent.MaxPixelCount)
            return;

        _drafts[uid] = new CanvasDesignDraft((uint[]) pixels.Clone(), name, description);
    }

    /// <summary>
    /// Attempts to retrieve an unsaved client-side draft associated with an entity. Returns true if a draft exists, false otherwise.
    /// </summary>
    public bool TryGetDraft(EntityUid uid, out CanvasDesignDraft draft)
    {
        return _drafts.TryGetValue(uid, out draft!);
    }

    /// <summary>
    /// Discards the unsaved client-side draft associated with an entity.
    /// </summary>
    public void ClearDraft(EntityUid uid)
    {
        _drafts.Remove(uid);
    }

    // When a canvas design entity is deleted, discard any unsaved draft associated with it.
    private void OnTerminating(Entity<CanvasDesignComponent> ent, ref EntityTerminatingEvent args)
    {
        _drafts.Remove(ent.Owner);
    }
}

/// <summary>
/// Unsaved editor contents retained locally when an editor closes unexpectedly.
/// </summary>
public sealed record CanvasDesignDraft(uint[] Pixels, string Name, string Description);

/// <summary>
/// Raised when a user attempts to preview their changes locally. The event contains the pixel array to be previewed,
/// which will be applied to the canvas texture until the preview ends or is cancelled.
/// </summary>
[ByRefEvent]
public readonly record struct CanvasDesignLocalPreviewEvent(uint[] Pixels);

/// <summary>
/// Raised when a user ends their local preview of changes. The canvas texture will be reverted to the authoritative state from the component's pixel array.
/// </summary>
[ByRefEvent]
public readonly record struct CanvasDesignLocalPreviewEndedEvent;
