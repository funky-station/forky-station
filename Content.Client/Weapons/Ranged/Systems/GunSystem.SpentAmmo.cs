// SPDX-FileCopyrightText: 2026 mq
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 PoorMansDreams
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Client.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.GameObjects;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    private void InitializeSpentAmmo()
    {
        SubscribeLocalEvent<SpentAmmoVisualsComponent, AppearanceChangeEvent>(OnSpentAmmoAppearance);
    }

    private void OnSpentAmmoAppearance(Entity<SpentAmmoVisualsComponent> ent, ref AppearanceChangeEvent args)
    {
        var sprite = args.Sprite;
        if (sprite == null) return;

        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Spent, out var varSpent))
        {
            return;
        }

        var spent = (bool)varSpent;
        string state;

        if (spent)
            state = ent.Comp.Suffix ? $"{ent.Comp.State}-spent" : "spent";
        else
            state = ent.Comp.State;

        _sprite.LayerSetRsiState((ent, sprite), AmmoVisualLayers.Base, state);
        _sprite.RemoveLayer((ent, sprite), AmmoVisualLayers.Tip, false);
    }
}
