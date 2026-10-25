using Robust.Shared.GameStates;

namespace Content.Server._Funkystation.EntitySystems.ParacusiaStatusEffect;

/// <summary>
/// Gives the user paracusia while active
/// </summary>
[NetworkedComponent, RegisterComponent]
public sealed partial class ParacusiaStatusEffectComponent : Component
{
    /// <summary>
    /// Should this go away when the status effect finishes
    /// </summary>
    [DataField]
    public bool RemovedOnEffectEnd = true;
}


