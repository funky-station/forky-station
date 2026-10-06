using Content.Shared._Funkystation.FurnitureWheels;
using Robust.Client.GameObjects;

namespace Content.Client._Funkystation.FurnitureWheels;

/// <summary>
/// Scaffolding for a locked/unlocked wheels sprite overlay. Silently no-ops if
/// the entity's sprite doesn't declare a "wheels" layer, so it's safe to attach
/// to any furniture before art exists — sprite artists add the layer + states
/// (<c>wheels_locked</c>, <c>wheels_unlocked</c>) and it lights up automatically.
/// </summary>
public sealed class FurnitureWheelsVisualizerSystem : VisualizerSystem<FurnitureWheelsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, FurnitureWheelsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite is not { } sprite)
            return;

        if (!AppearanceSystem.TryGetData<bool>(uid, FurnitureWheelsVisuals.Locked, out var locked, args.Component))
            return;

        if (!sprite.LayerMapTryGet(FurnitureWheelsVisualLayers.Wheels, out var layer))
            return;

        sprite.LayerSetState(layer, locked ? "wheels_locked" : "wheels_unlocked");
        sprite.LayerSetVisible(layer, true);
    }
}

public enum FurnitureWheelsVisualLayers : byte
{
    Wheels
}
