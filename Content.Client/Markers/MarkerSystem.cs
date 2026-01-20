// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Vince
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Markers;

public sealed class MarkerSystem : EntitySystem
{
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private bool _markersVisible;

    public bool MarkersVisible
    {
        get => _markersVisible;
        set
        {
            _markersVisible = value;
            UpdateMarkers();
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MarkerComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, MarkerComponent marker, ComponentStartup args)
    {
        UpdateVisibility(uid);
    }

    private void UpdateVisibility(EntityUid uid)
    {
        if (TryComp(uid, out SpriteComponent? sprite))
        {
            _sprite.SetVisible((uid, sprite), MarkersVisible);
        }
    }

    private void UpdateMarkers()
    {
        var query = AllEntityQuery<MarkerComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            UpdateVisibility(uid);
        }
    }
}
