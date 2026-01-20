// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-FileCopyrightText: 2024 SlamBamActionman
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Willhelm53
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids.Components;
using Robust.Shared.Map;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System.Linq;

namespace Content.Server.Chemistry.TileReactions;

/// <summary>
/// Turns all of the reagents on a puddle into water.
/// </summary>
[DataDefinition]
public sealed partial class CleanTileReaction : ITileReaction
{
    /// <summary>
    /// How much it costs to clean 1 unit of reagent.
    /// </summary>
    /// <remarks>
    /// In terms of space cleaner can clean 1 average puddle per 5 units.
    /// </remarks>
    [DataField("cleanCost")]
    public float CleanAmountMultiplier { get; private set; } = 0.25f;

    /// <summary>
    /// What reagent to replace the tile conents with.
    /// </summary>
    [DataField("reagent", customTypeSerializer: typeof(PrototypeIdSerializer<ReagentPrototype>))]
    public string ReplacementReagent = "Water";

    FixedPoint2 ITileReaction.TileReact(TileRef tile,
        ReagentPrototype reagent,
        FixedPoint2 reactVolume,
        IEntityManager entityManager
        , List<ReagentData>? data)
    {
        var entities = entityManager.System<EntityLookupSystem>().GetLocalEntitiesIntersecting(tile, 0f).ToArray();
        var puddleQuery = entityManager.GetEntityQuery<PuddleComponent>();
        var solutionContainerSystem = entityManager.System<SharedSolutionContainerSystem>();
        // Multiply as the amount we can actually purge is higher than the react amount.
        var purgeAmount = reactVolume / CleanAmountMultiplier;

        foreach (var entity in entities)
        {
            if (!puddleQuery.TryGetComponent(entity, out var puddle) ||
                !solutionContainerSystem.TryGetSolution(entity, puddle.SolutionName, out var puddleSolution, out _))
            {
                continue;
            }

            var purgeable = solutionContainerSystem.SplitSolutionWithout(puddleSolution.Value, purgeAmount, ReplacementReagent, reagent.ID);

            purgeAmount -= purgeable.Volume;

            solutionContainerSystem.TryAddSolution(puddleSolution.Value, new Solution(ReplacementReagent, purgeable.Volume));

            if (purgeable.Volume <= FixedPoint2.Zero)
                break;
        }

        return (reactVolume / CleanAmountMultiplier - purgeAmount) * CleanAmountMultiplier;
    }
}
