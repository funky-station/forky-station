using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Reagent;

/// <summary>
/// ReagentData that tracks how a reagent entered the body.
/// When the same reagent is administered via different routes, each instance is stored
/// as a separate entry in a solution, allowing route-specific metabolic effects to apply
/// independently and not stack with one another.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class AdministrationRouteData : ReagentData
{
    /// <summary>
    /// The route through which this reagent was administered.
    /// </summary>
    [DataField]
    public ReagentAdministrationRoute Route;

    public AdministrationRouteData() { }

    public AdministrationRouteData(ReagentAdministrationRoute route)
    {
        Route = route;
    }

    public override bool Equals(ReagentData? other)
    {
        return other is AdministrationRouteData routeData && routeData.Route == Route;
    }

    public override int GetHashCode()
    {
        return Route.GetHashCode();
    }

    public override ReagentData Clone()
    {
        return new AdministrationRouteData(Route);
    }

    public override string ToString(string prototype, FixedPoint2 quantity)
    {
        return $"{prototype}:{Route}:{quantity}";
    }

    public override string ToString(string prototype)
    {
        return $"{prototype}:{Route}";
    }
}
