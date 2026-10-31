namespace Content.Shared._Impstation.PersonalEconomy.Components;

/// <summary>
/// This is used to mark things as ATMs.
/// </summary>
[RegisterComponent]
public sealed partial class ATMComponent : Component
{
    // slot the bank card goes in
    [DataField]
    public string CardSlotId = "card_slot";
}
