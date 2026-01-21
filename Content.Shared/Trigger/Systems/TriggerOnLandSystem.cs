// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Studio Fae-Wilds <studio.faewilds@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Throwing;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

public sealed partial class TriggerOnLandSystem : TriggerOnXSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnLandComponent, LandEvent>(OnLand);
    }

    private void OnLand(Entity<TriggerOnLandComponent> ent, ref LandEvent args)
    {
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
    }
}
