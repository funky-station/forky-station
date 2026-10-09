using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

[Serializable, NetSerializable]
public sealed class PoSTransactionSuccededMessage : BoundUserInterfaceMessage;
