using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Clothing.Components;

/// <summary>
/// Slows stamina regen while this item is worn
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ClothingStaminaRegenModifierComponent : Component
{
    /// <summary>
    /// multiplied into stamina regen while worn. 1 = no change, lower = slower regen.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float RegenCoefficient = 1f;
}
