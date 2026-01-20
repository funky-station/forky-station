// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Steven K
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Shared.Botany;
using Content.Client.Botany.Components;
using Robust.Client.GameObjects;

namespace Content.Client.Botany;

public sealed class PotencyVisualsSystem : VisualizerSystem<PotencyVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, PotencyVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (AppearanceSystem.TryGetData<float>(uid, ProduceVisuals.Potency, out var potency, args.Component))
        {
            var scale = MathHelper.Lerp(component.MinimumScale, component.MaximumScale, potency / 100);
            SpriteSystem.SetScale((uid, args.Sprite), new Vector2(scale, scale));
        }
    }
}
