// SPDX-FileCopyrightText: 2025 Nikovnik <116634167+nkokic@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects.Body;

/// <summary>
/// Removes a given amount of chemicals from the bloodstream modified by scale.
/// Optionally ignores a given chemical.
/// </summary>
/// <inheritdoc cref="EntityEffectSystem{T,TEffect}"/>
public sealed partial class CleanBloodstreamEntityEffectSystem : EntityEffectSystem<BloodstreamComponent, CleanBloodstream>
{
    [Dependency] private readonly SharedBloodstreamSystem _bloodstream = default!;

    protected override void Effect(Entity<BloodstreamComponent> entity, ref EntityEffectEvent<CleanBloodstream> args)
    {
        var scale = args.Scale * args.Effect.CleanseRate;

        _bloodstream.FlushChemicals((entity, entity), scale, args.Effect.Excluded);
    }
}

/// <inheritdoc cref="EntityEffect"/>
public sealed partial class CleanBloodstream : EntityEffectBase<CleanBloodstream>
{
    /// <summary>
    ///     Amount of reagent we're cleaning out of our bloodstream.
    /// </summary>
    [DataField]
    public FixedPoint2 CleanseRate = 3.0f;

    /// <summary>
    ///     An optional chemical to ignore when doing removal.
    /// </summary>
    [DataField]
    public ProtoId<ReagentPrototype>? Excluded;

    public override string EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("entity-effect-guidebook-clean-bloodstream", ("chance", Probability));
}
