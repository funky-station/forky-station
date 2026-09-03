using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.SuitBreach.Components;

/// <summary>
/// goes on a suit to make it able to breach
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SuitBreachableComponent : Component
{
    /// <summary>
    /// 0-1 chance that a hit punctures the suit. rolled per hit
    /// </summary>
    [DataField, AutoNetworkedField]
    public float BreachChance = 0.5f;

    /// <summary>
    /// minimum damage to roll a breach at all
    /// </summary>
    [DataField, AutoNetworkedField]
    public float MinDamageToRoll = 5f;

    /// <summary>
    /// you can set this to false in order to make a suit sabotage proof
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool SabotageAlwaysBreaches = true;

    /// <summary>
    /// how long it takes to sabotage a suit
    /// </summary>
    [DataField]
    public TimeSpan SabotageDelay = TimeSpan.FromSeconds(10);
}
