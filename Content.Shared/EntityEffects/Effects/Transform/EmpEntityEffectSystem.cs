// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-License-Identifier: MIT

using Content.Shared.Database;
using Content.Shared.Emp;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects.Transform;

/// <summary>
/// Creates an EMP at this entity's coordinates.
/// Range is modified by scale.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class EmpEntityEffectSystem : EntityEffectSystem<TransformComponent, Emp>
{
    [Dependency] private readonly SharedEmpSystem _emp = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;

    protected override void Effect(Entity<TransformComponent> entity, ref EntityEffectEvent<Emp> args)
    {
        var range = MathF.Min(args.Effect.RangeModifier * args.Scale, args.Effect.MaxRange);

        _emp.EmpPulse(_xform.GetMapCoordinates(entity, xform: entity.Comp), range, args.Effect.EnergyConsumption, args.Effect.Duration);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class Emp : EntityEffectBase<Emp>
{
    /// <summary>
    ///     Impulse range per unit of quantity
    /// </summary>
    [DataField]
    public float RangeModifier = 0.5f;

    /// <summary>
    ///     Maximum impulse range
    /// </summary>
    [DataField]
    public float MaxRange = 10;

    /// <summary>
    ///     How much energy will be drain from sources
    /// </summary>
    [DataField]
    public float EnergyConsumption = 12500;

    /// <summary>
    ///     Amount of time entities will be disabled
    /// </summary>
    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(15);

    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("entity-effect-guidebook-emp-reaction-effect", ("chance", Probability));

    public override LogImpact? Impact => LogImpact.Medium;
}
