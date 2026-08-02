using Content.Shared.Inventory;
using Robust.Shared.GameStates;

namespace Content.Shared._ES.Viewcone.Components;

/// <summary>
///     Intended to be used on inventory items or status effects (i.e. this is relayed).
///     Modifies the viewcone angle of the relevant entity additively.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ESViewconeModifierComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public float AngleModifier = 0f;
}

/// <summary>
///     Raised clientside by-ref and broadcast on an entity with a viewcone, and relayed to inventory & status effects.
///     Modifies their viewcone angle additively.
/// </summary>
[ByRefEvent]
public record ESViewconeGetAngleModifierEvent : IInventoryRelayEvent
{
    public SlotFlags TargetSlots => SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK;

    private float _angleModifier;

    public float GetAngleModifier()
    {
        return _angleModifier;
    }

    public void ModifyAngle(float angle)
    {
        _angleModifier += angle;
    }
}
