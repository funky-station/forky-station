// SPDX-FileCopyrightText: 2024 Kirus59
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2024 Krunklehorn
// SPDX-FileCopyrightText: 2023 PrPleGoo
// SPDX-FileCopyrightText: 2024 PrPleGoo
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-License-Identifier: MIT

using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon.Components;

namespace Content.Client.Overlays;

public sealed class ShowThirstIconsSystem : EquipmentHudSystem<ShowThirstIconsComponent>
{
    [Dependency] private readonly ThirstSystem _thirst = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ThirstComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, ThirstComponent component, ref GetStatusIconsEvent ev)
    {
        if (!IsActive)
            return;

        if (_thirst.TryGetStatusIconPrototype(component, out var iconPrototype))
            ev.StatusIcons.Add(iconPrototype);
    }
}
