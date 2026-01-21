// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Systems;

namespace Content.Shared.Damage.Components;

/// <summary>
/// Makes this entity deal damage when thrown at something.
/// </summary>
[RegisterComponent]
[Access(typeof(SharedDamageOtherOnHitSystem))]
public sealed partial class DamageOtherOnHitComponent : Component
{
    /// <summary>
    /// Whether to ignore damage modifiers.
    /// </summary>
    [DataField]
    public bool IgnoreResistances = false;

    /// <summary>
    /// The damage amount to deal on hit.
    /// </summary>
    [DataField(required: true)]
    public DamageSpecifier Damage = default!;

}
