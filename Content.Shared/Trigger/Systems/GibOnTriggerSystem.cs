// SPDX-FileCopyrightText: 2026 pathetic meowmeow
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-License-Identifier: MIT

using Content.Shared.Gibbing;
using Content.Shared.Inventory;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

public sealed class GibOnTriggerSystem : XOnTriggerSystem<GibOnTriggerComponent>
{
    [Dependency] private readonly GibbingSystem _gibbing = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;

    protected override void OnTrigger(Entity<GibOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (ent.Comp.DeleteItems)
        {
            var items = _inventory.GetHandOrInventoryEntities(target);
            foreach (var item in items)
            {
                PredictedQueueDel(item);
            }
        }

        _gibbing.Gib(target, user: args.User);
        args.Handled = true;
    }
}
