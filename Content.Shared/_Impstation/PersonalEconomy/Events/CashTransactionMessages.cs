using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

// withdraw physical scrip from the account in the ATM slot; requires the account PIN
[Serializable, NetSerializable]
public sealed class WithdrawMessage(int amount, int pin) : BoundUserInterfaceMessage
{
    public int Amount = amount;
    public int Pin = pin;
}

// deposit the scrip the player is holding into the account in the ATM slot
[Serializable, NetSerializable]
public sealed class DepositMessage : BoundUserInterfaceMessage;
