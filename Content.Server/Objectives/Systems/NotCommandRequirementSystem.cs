// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Theodore Lukin <66275205+pheenty@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Objectives.Components;
using Content.Server.Revolutionary.Components;
using Content.Shared.Objectives.Components;

namespace Content.Server.Objectives.Systems;

public sealed class NotCommandRequirementSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<NotCommandRequirementComponent, RequirementCheckEvent>(OnCheck);
    }

    private void OnCheck(EntityUid uid, NotCommandRequirementComponent comp, ref RequirementCheckEvent args)
    {
        if (args.Cancelled)
            return;

        if (args.Mind.OwnedEntity is { } ent && HasComp<CommandStaffComponent>(ent))
            args.Cancelled = true;
    }
}
