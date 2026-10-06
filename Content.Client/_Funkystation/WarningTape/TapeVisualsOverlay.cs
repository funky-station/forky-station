using System.Numerics;
using Content.Shared._Funkystation.WarningTape.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._Funkystation.WarningTape;

/// <summary>
/// draws placed tape
/// </summary>
public sealed class TapeVisualsOverlay(IEntityManager entManager, IPrototypeManager protoManager) : Overlay
{
    public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowFOV;

    protected override void Draw(in OverlayDrawArgs args)
    {
        var worldHandle = args.WorldHandle;
        var spriteSystem = entManager.System<SpriteSystem>();
        var xformSystem = entManager.System<SharedTransformSystem>();

        args.DrawingHandle.SetTransform(Matrix3x2.Identity);

        var tapes = entManager.EntityQueryEnumerator<TapeVisualsComponent, TransformComponent>();
        while (tapes.MoveNext(out var visuals, out var xform))
        {
            if (xform.MapID != args.MapId)
                continue;

            var posA = xformSystem.ToMapCoordinates(xform.Coordinates).Position;
            var posB = xformSystem.ToMapCoordinates(visuals.End).Position;

            DrawTapeLine(worldHandle, spriteSystem, visuals.Sprite, visuals.ContentRegion, posA, posB, visuals.TileCount);
        }

        var rolls = entManager.EntityQueryEnumerator<TapeRollComponent, TransformComponent>();
        while (rolls.MoveNext(out var roll, out var rollXform))
        {
            if (roll.PendingStart is not { } startCoords || rollXform.MapID != args.MapId)
                continue;

            var startMap = xformSystem.ToMapCoordinates(startCoords);
            if (startMap.MapId != args.MapId)
                continue;

            var posA = startMap.Position;

            var userPos = roll.PendingUser != null && entManager.TryGetComponent<TransformComponent>(roll.PendingUser, out var userXform)
                ? xformSystem.ToMapCoordinates(userXform.Coordinates).Position
                : xformSystem.ToMapCoordinates(rollXform.Coordinates).Position;

            if (!protoManager.TryIndex<EntityPrototype>(roll.TapePrototype, out var proto) ||
                !proto.TryComp<TapeVisualsComponent>(out var protoVisuals, entManager.ComponentFactory))
            {
                continue;
            }

            var diff = userPos - posA;
            var length = diff.Length();
            var tileLen = protoVisuals.ContentRegion.Width / EyeManager.PixelsPerMeter;
            var tileCount = Math.Max(1, (int) MathF.Round(length / tileLen));

            DrawTapeLine(worldHandle, spriteSystem, protoVisuals.Sprite, protoVisuals.ContentRegion, posA, userPos, tileCount, alpha: 0.85f);
        }
    }

    private void DrawTapeLine(
        DrawingHandleWorld worldHandle,
        SpriteSystem spriteSystem,
        SpriteSpecifier spriteSpec,
        Box2 contentRegion,
        Vector2 posA,
        Vector2 posB,
        int tileCount,
        float alpha = 1.0f)
    {
        var texture = spriteSystem.Frame0(spriteSpec);
        var tileLen = contentRegion.Width / EyeManager.PixelsPerMeter;
        var thickness = contentRegion.Height / EyeManager.PixelsPerMeter;

        var subRegion = new UIBox2(
            contentRegion.Left,
            Math.Min(contentRegion.Top, contentRegion.Bottom),
            contentRegion.Right,
            Math.Max(contentRegion.Top, contentRegion.Bottom));

        var diff = posB - posA;
        var length = diff.Length();

        if (length < 0.01f || tileLen < 0.01f)
            return;

        var direction = diff / length;
        var angle = new Angle(Math.Atan2(diff.Y, diff.X));
        var modulate = Color.White.WithAlpha(alpha);

        for (var i = 0; i < tileCount; i++)
        {
            var centerDist = i * tileLen + tileLen / 2f;
            var tileMid = posA + direction * centerDist;

            var box = new Box2(-tileLen / 2f, -thickness / 2f, tileLen / 2f, thickness / 2f);
            var rotated = new Box2Rotated(box.Translated(tileMid), angle, tileMid);
            worldHandle.DrawTextureRectRegion(texture, rotated, subRegion: subRegion, modulate: modulate);
        }
    }
}
