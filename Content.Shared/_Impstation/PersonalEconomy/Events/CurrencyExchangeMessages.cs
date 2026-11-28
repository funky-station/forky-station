using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

// insert the cash stack the player is holding into the exchange
[Serializable, NetSerializable]
public sealed class InsertCashMessage : BoundUserInterfaceMessage;

// eject the cash currently in the exchange back to the player
[Serializable, NetSerializable]
public sealed class EjectCashMessage : BoundUserInterfaceMessage;

// convert the inserted cash to the other currency, minus tax
[Serializable, NetSerializable]
public sealed class ConvertCurrencyMessage : BoundUserInterfaceMessage;
