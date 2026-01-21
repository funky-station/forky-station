// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Studio Fae-Wilds <studio.faewilds@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Tools.Components;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

public sealed class TriggerOnToolUseSystem : TriggerOnXSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnSimpleToolUsageComponent, SimpleToolDoAfterEvent>(OnToolUse);
    }

    private void OnToolUse(Entity<TriggerOnSimpleToolUsageComponent> ent, ref SimpleToolDoAfterEvent args)
    {
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
    }
}
