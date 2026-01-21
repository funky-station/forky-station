// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Fishfish458 <47410468+Fishfish458@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Client.GameObjects;

using static Content.Shared.Paper.PaperComponent;

namespace Content.Client.Paper.UI;

public sealed class PaperVisualizerSystem : VisualizerSystem<PaperVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, PaperVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (AppearanceSystem.TryGetData<PaperStatus>(uid, PaperVisuals.Status, out var writingStatus, args.Component))
            SpriteSystem.LayerSetVisible((uid, args.Sprite), PaperVisualLayers.Writing, writingStatus == PaperStatus.Written);

        if (AppearanceSystem.TryGetData<string>(uid, PaperVisuals.Stamp, out var stampState, args.Component))
        {
            if (stampState != string.Empty)
            {
                SpriteSystem.LayerSetRsiState((uid, args.Sprite), PaperVisualLayers.Stamp, stampState);
                SpriteSystem.LayerSetVisible((uid, args.Sprite), PaperVisualLayers.Stamp, true);
            }
            else
            {
                SpriteSystem.LayerSetVisible((uid, args.Sprite), PaperVisualLayers.Stamp, false);
            }

        }
    }
}

public enum PaperVisualLayers
{
    Stamp,
    Writing
}
