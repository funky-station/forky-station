using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Documents;

[Serializable, NetSerializable]
public enum DocumentPrinterUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class DocumentEntry(
    string id,
    string name,
    string description,
    bool accessible,
    List<string> requiredAccess)
{
    public readonly string Id = id;
    public readonly string Name = name;
    public readonly string Description = description;
    public readonly bool Accessible = accessible;
    public readonly List<string> RequiredAccess = requiredAccess;
}

[Serializable, NetSerializable]
public sealed class DocumentPrinterBoundUserInterfaceState(
    Dictionary<string, List<DocumentEntry>> documentsByCategory,
    bool isPaperInserted,
    string? insertedPaperName,
    bool canCopy,
    bool canPrint,
    bool isJammed)
    : BoundUserInterfaceState
{
    public readonly Dictionary<string, List<DocumentEntry>> DocumentsByCategory = documentsByCategory;
    public readonly bool IsPaperInserted = isPaperInserted;
    public readonly string? InsertedPaperName = insertedPaperName;
    public readonly bool CanCopy = canCopy;
    public readonly bool CanPrint = canPrint;
    public readonly bool IsJammed = isJammed;
}

[Serializable, NetSerializable]
public sealed class DocumentPrinterPrintMessage(string documentId) : BoundUserInterfaceMessage
{
    public readonly string DocumentId = documentId;
}

/// <summary>
/// Copy whatever paper is currently in the printer's slot
/// </summary>
[Serializable, NetSerializable]
public sealed class DocumentPrinterCopyMessage : BoundUserInterfaceMessage;

/// <summary>
/// Tells player to eject whatever paper is in its slot
/// </summary>
[Serializable, NetSerializable]
public sealed class DocumentPrinterEjectMessage : BoundUserInterfaceMessage;
