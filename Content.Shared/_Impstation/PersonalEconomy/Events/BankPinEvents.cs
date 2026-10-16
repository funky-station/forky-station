using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

[Serializable, NetSerializable]
public sealed class RequestBankPinEvent : EntityEventArgs;

[Serializable, NetSerializable]
public sealed class BankPinResponseEvent(int accountNumber, int pin) : EntityEventArgs
{
    public int AccountNumber = accountNumber;
    public int Pin = pin;
}
