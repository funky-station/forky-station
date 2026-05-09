using Content.Shared.Chemistry.Components;
using Content.Shared.Kitchen.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.FixedPoint;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Content.Shared.Chemistry.Reagent;

namespace Content.Shared.Chemistry.EntitySystems;

public abstract partial class SharedSolutionContainerSystem
{
    #region Solution Accessors

    public bool TryGetRefillableSolution(Entity<RefillableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    }

    public bool TryGetDrainableSolution(Entity<DrainableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    }

    public bool TryGetExtractableSolution(Entity<ExtractableComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.GrindableSolution, out soln, out solution);
    }

    public bool TryGetDumpableSolution(Entity<DumpableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    }

    public bool TryGetDrawableSolution(Entity<DrawableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    }

    public bool TryGetInjectableSolution(Entity<InjectableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    }

    public bool TryGetFitsInDispenser(Entity<FitsInDispenserComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    }

    public bool TryGetMixableSolution(Entity<MixableSolutionComponent?, SolutionContainerManagerComponent?> entity, [NotNullWhen(true)] out Entity<SolutionComponent>? soln, [NotNullWhen(true)] out Solution? solution)
    {
        if (!Resolve(entity, ref entity.Comp1, logMissing: false))
        {
            (soln, solution) = (default!, null);
            return false;
        }

        return TryGetSolution((entity.Owner, entity.Comp2), entity.Comp1.Solution, out soln, out solution);
    }

    #endregion Solution Accessors

    #region Solution Modifiers

    public void Refill(Entity<RefillableSolutionComponent?> entity, Entity<SolutionComponent> soln, Solution refill)
    {
        if (!Resolve(entity, ref entity.Comp, logMissing: false))
            return;

        AddSolution(soln, refill);
    }

    public void Inject(Entity<InjectableSolutionComponent?> entity, Entity<SolutionComponent> soln, Solution inject)
    {
        if (!Resolve(entity, ref entity.Comp, logMissing: false))
            return;

        AddSolution(soln, inject);
    }

    public Solution Drain(Entity<DrainableSolutionComponent?> entity, Entity<SolutionComponent> soln, FixedPoint2 quantity)
    {
        if (!Resolve(entity, ref entity.Comp, logMissing: false))
            return new();

        return SplitSolution(soln, quantity);
    }

    public Solution Draw(Entity<DrawableSolutionComponent?> entity, Entity<SolutionComponent> soln, FixedPoint2 quantity)
    {
        if (!Resolve(entity, ref entity.Comp, logMissing: false))
            return new();

        return SplitSolution(soln, quantity);
    }

    #endregion Solution Modifiers

    /// <returns>A value between 0 and 100 inclusive.</returns>
    public float PercentFull(EntityUid uid)
    {
        if (!TryGetDrainableSolution(uid, out _, out var solution))
            return 0;

        return PercentFull(solution);
    }

    #region Static Methods

    public static string ToPrettyString(Solution solution)
    {
        var sb = new StringBuilder();
        if (solution.Name == null)
            sb.Append("[");
        else
            sb.Append($"{solution.Name}:[");
        var first = true;
        foreach (var (id, quantity) in solution.Contents)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append(", ");
            }

            sb.AppendFormat("{0}: {1}u", id, quantity);
        }

        sb.Append(']');
        return sb.ToString();
    }

    /// <returns>A value between 0 and 100 inclusive.</returns>
    public static float PercentFull(Solution sol)
    {
        if (sol.MaxVolume.Equals(FixedPoint2.Zero))
            return 0;

        return sol.FillFraction * 100;
    }

    /// <summary>
    /// Tags all reagents in a solution with a specific administration route by adding
    /// <see cref="AdministrationRouteData"/> to each reagent's data list.
    /// Call this on a split solution immediately before it is added to a body, so that
    /// route-specific metabolisms apply correctly.
    /// </summary>
    /// <remarks>
    /// The same reagent administered via two different routes will appear as two separate
    /// entries in the destination solution, preserving their individual effects.
    /// If <paramref name="route"/> is <see cref="ReagentAdministrationRoute.Unspecified"/>,
    /// this method does nothing.
    /// </remarks>
    /// <param name="solution">The solution whose reagents will be tagged.</param>
    /// <param name="route">The administration route to apply.</param>
    public static void SetAdministrationRoute(Solution solution, ReagentAdministrationRoute route)
    {
        if (route == ReagentAdministrationRoute.Unspecified)
            return;

        var routeData = new AdministrationRouteData(route);

        for (var i = 0; i < solution.Contents.Count; i++)
        {
            var quantity = solution.Contents[i];

            // Copy existing data list, or start a fresh one.
            var data = quantity.Reagent.Data != null
                ? new List<ReagentData>(quantity.Reagent.Data)
                : new List<ReagentData>(1);

            // Replace any pre-existing route entry so we don't accumulate stale entries.
            data.RemoveAll(d => d is AdministrationRouteData);
            data.Add(routeData);

            solution.Contents[i] = new ReagentQuantity(
                new ReagentId(quantity.Reagent.Prototype, data),
                quantity.Quantity);
        }
    }

    #endregion Static Methods
}
