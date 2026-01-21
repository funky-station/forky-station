// SPDX-FileCopyrightText: 2023-2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Roles.Components;

namespace Content.Server.Roles;

public sealed class RoleBriefingSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoleBriefingComponent, GetBriefingEvent>(OnGetBriefing);
    }

    private void OnGetBriefing(EntityUid uid, RoleBriefingComponent comp, ref GetBriefingEvent args)
    {
        args.Append(Loc.GetString(comp.Briefing));
    }
}
