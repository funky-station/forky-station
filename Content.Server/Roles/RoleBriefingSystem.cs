// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2024 deltanedas
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
