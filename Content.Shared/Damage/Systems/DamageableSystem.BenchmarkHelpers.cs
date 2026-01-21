// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Components;

namespace Content.Shared.Damage.Systems;

public sealed partial class DamageableSystem
{
    /// <summary>
    /// Applies damage to all entities to see how expensive it is to deal damage.
    /// </summary>
    public void ApplyDamageToAllEntities(List<Entity<DamageableComponent>> damageables, DamageSpecifier damage)
    {
        foreach (var (uid, damageable) in damageables)
        {
            ChangeDamage((uid, damageable), damage);
        }
    }
}
