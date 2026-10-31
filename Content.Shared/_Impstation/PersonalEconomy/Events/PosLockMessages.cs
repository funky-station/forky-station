using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

// try to unlock merchant configuration when we want to edit
[Serializable, NetSerializable]
public sealed class UnlockPosMessage(int pin) : BoundUserInterfaceMessage
{
    public int Pin = pin;
}

// pin was correct so we switch to the merchant view
[Serializable, NetSerializable]
public sealed class PosUnlockedMessage : BoundUserInterfaceMessage;
