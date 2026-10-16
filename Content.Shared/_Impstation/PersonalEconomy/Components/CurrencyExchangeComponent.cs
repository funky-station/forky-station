using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.PersonalEconomy.Components;

/// <summary>
/// A computer that converts spesos to scrip and vice versa
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CurrencyExchangeComponent : Component
{
    // slot the cash to convert goes in
    [DataField]
    public string CashSlotId = "cash_slot";

    // percentage skimmed off each conversion
    [DataField, AutoNetworkedField]
    public int TaxPercent = 10;

    // how many scrip one speso is worth
    [DataField, AutoNetworkedField]
    public int ScripPerSpeso = 30;

    public static int ComputeConversion(int count, bool inputIsScrip, int taxPercent, int scripPerSpeso)
    {
        var afterTax = 100 - taxPercent;
        return inputIsScrip
            ? count * afterTax / (100 * scripPerSpeso) // scrip 2 spesos
            : count * scripPerSpeso * afterTax / 100;   // spesos 2 scrip
    }
}
