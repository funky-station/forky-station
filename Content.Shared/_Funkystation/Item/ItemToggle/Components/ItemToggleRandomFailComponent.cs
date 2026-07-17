using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Item.ItemToggle.Components;

/// <summary>
/// Handles random fail events when attempting to activate an item.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ItemToggleRandomFailComponent : Component
{
    /// <summary>
    /// The chance the item will fail to toggle on, between 0.0 and 1.0.
    /// </summary>
    [DataField]
    public float FailChance = 0f;

    /// <summary>
    /// Localization ID for text to display when the item fails to activate.
    /// </summary>
    [DataField]
    public LocId? PopupText;
}
