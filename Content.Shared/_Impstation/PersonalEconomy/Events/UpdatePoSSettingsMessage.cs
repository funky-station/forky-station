using Content.Shared._Impstation.PersonalEconomy.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

[Serializable, NetSerializable]
public sealed class UpdatePoSSettingsMessage(AccountNumber recipient, int amount, string reason, string merchantName) : BoundUserInterfaceMessage
{
    public AccountNumber Recipient = recipient;
    public int Amount = amount;
    public string Reason = reason;
    public string MerchantName = merchantName;
}

[Serializable, NetSerializable]
public sealed class PoSTipMessage(int amount) : BoundUserInterfaceMessage
{
    public int Amount = amount;
}
