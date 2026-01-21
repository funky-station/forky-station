// SPDX-FileCopyrightText: 2025 Samuka <47865393+Samuka-C@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects.Damage;

/// <summary>
/// Adjust the damages on this entity by specified amounts.
/// Amounts are modified by scale.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class HealthChangeEntityEffectSystem : EntityEffectSystem<DamageableComponent, HealthChange>
{
    [Dependency] private readonly DamageableSystem _damageable = default!;

    protected override void Effect(Entity<DamageableComponent> entity, ref EntityEffectEvent<HealthChange> args)
    {
        var damageSpec = new DamageSpecifier(args.Effect.Damage);

        damageSpec *= args.Scale;

        _damageable.TryChangeDamage(
                entity.AsNullable(),
                damageSpec,
                args.Effect.IgnoreResistances,
                interruptsDoAfters: false);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class HealthChange : EntityEffectBase<HealthChange>
{
    /// <summary>
    /// Damage to apply every cycle. Damage Ignores resistances.
    /// </summary>
    [DataField(required: true)]
    public DamageSpecifier Damage = default!;

    [DataField]
    public bool IgnoreResistances = true;

    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        {
            var damages = new List<string>();
            var heals = false;
            var deals = false;

            var damageSpec = new DamageSpecifier(Damage);

            var universalReagentDamageModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentDamageModifier;
            var universalReagentHealModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentHealModifier;

            damageSpec = entSys.GetEntitySystem<DamageableSystem>().ApplyUniversalAllModifiers(damageSpec);

            foreach (var (kind, amount) in damageSpec.DamageDict)
            {
                var sign = FixedPoint2.Sign(amount);
                float mod;

                switch (sign)
                {
                    case < 0:
                        heals = true;
                        mod = universalReagentHealModifier;
                        break;
                    case > 0:
                        deals = true;
                        mod = universalReagentDamageModifier;
                        break;
                    default:
                        continue; // Don't need to show damage types of 0...
                }

                damages.Add(
                    Loc.GetString("health-change-display",
                        ("kind", prototype.Index<DamageTypePrototype>(kind).LocalizedName),
                        ("amount", MathF.Abs(amount.Float() * mod)),
                        ("deltasign", sign)
                    ));
            }

            var healsordeals = heals ? (deals ? "both" : "heals") : (deals ? "deals" : "none");

            return Loc.GetString("entity-effect-guidebook-health-change",
                ("chance", Probability),
                ("changes", ContentLocalizationManager.FormatList(damages)),
                ("healsordeals", healsordeals));
        }
}
