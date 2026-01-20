// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Robust.Client.GameObjects;

namespace Content.Client.Storage.Visualizers;

public sealed class StorageFillVisualizerSystem : VisualizerSystem<StorageFillVisualizerComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, StorageFillVisualizerComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!AppearanceSystem.TryGetData<int>(uid, StorageFillVisuals.FillLevel, out var level, args.Component))
            return;

        var state = $"{component.FillBaseName}-{level}";
        SpriteSystem.LayerSetRsiState((uid, args.Sprite), StorageFillLayers.Fill, state);
    }
}
