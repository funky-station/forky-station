// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Emp;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

public sealed class EmpOnTriggerSystem : XOnTriggerSystem<EmpOnTriggerComponent>
{
    [Dependency] private readonly SharedEmpSystem _emp = default!;

    protected override void OnTrigger(Entity<EmpOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        _emp.EmpPulse(Transform(target).Coordinates, ent.Comp.Range, ent.Comp.EnergyConsumption, ent.Comp.DisableDuration, args.User);
        args.Handled = true;
    }
}
