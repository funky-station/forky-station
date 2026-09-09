using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Documents;

[Serializable, NetSerializable]
public sealed partial class DocumentPrinterJamClearDoAfterEvent : SimpleDoAfterEvent
{
}
