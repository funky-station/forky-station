// SPDX-FileCopyrightText: 2025 ArtisticRoomba <145879011+ArtisticRoomba@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos.Components;
using Content.Shared.Examine;

namespace Content.Shared.Atmos.EntitySystems;

public abstract partial class SharedDeltaPressureSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DeltaPressureComponent, ExaminedEvent>(OnExaminedEvent);
    }

    private void OnExaminedEvent(Entity<DeltaPressureComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.IsTakingDamage)
            args.PushMarkup(Loc.GetString("window-taking-damage"));
    }
}
