// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2024 Krunklehorn
// SPDX-FileCopyrightText: 2024 PrPleGoo
// SPDX-FileCopyrightText: 2023 IntegerTempest
// SPDX-License-Identifier: MIT

using Content.Shared.Overlays;
using Content.Shared.NukeOps;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowSyndicateIconsSystem : EquipmentHudSystem<ShowSyndicateIconsComponent>
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NukeOperativeComponent, GetStatusIconsEvent>(OnGetStatusIconsEvent);
    }

    private void OnGetStatusIconsEvent(EntityUid uid, NukeOperativeComponent component, ref GetStatusIconsEvent ev)
    {
        if (!IsActive)
            return;

        if (_prototype.TryIndex<FactionIconPrototype>(component.SyndStatusIcon, out var iconPrototype))
            ev.StatusIcons.Add(iconPrototype);
    }
}
