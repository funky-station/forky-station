using Content.Shared.Ninja.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Ninja.Components;

/// <summary>
/// Component for the Space Ninja's unique Spider Charge.
/// Only this component detonating can trigger the ninja's objective.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedSpiderChargeSystem))]
[AutoGenerateComponentState] // funky
public sealed partial class SpiderChargeComponent : Component
{
    /// <summary>
    /// Range for planting within the target area.
    /// </summary>
    [DataField]
    public float Range = 13f; // funky change from 10f

    /// <summary>
    /// The ninja that planted this charge.
    /// </summary>
    [DataField]
    public EntityUid? Planter;

    /// <summary>
    /// The trigger that will mark the objective as successful.
    /// </summary>
    [DataField]
    public string TriggerKey = "timer";

    // funky
    /// <summary>
    /// If the charge is armed or not, used to prevent recalling an armed charge
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Armed = false;
}
