using Robust.Shared.Enums;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.StationRecords;

[Serializable, NetSerializable]
public enum XoRecordsConsoleKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class XoRecordListingEntry(uint id, string displayName, bool flagged)
{
    public readonly uint Id = id;
    public readonly string DisplayName = displayName;
    public readonly bool Flagged = flagged;
}

[Serializable, NetSerializable]
public sealed class XoRecordFields(
    string name,
    int age,
    string jobTitle,
    string species,
    Gender gender,
    string? fingerprint,
    string? dna,
    int? pagerNumber = null)
{
    public readonly string Name = name;
    public readonly int Age = age;
    public readonly string JobTitle = jobTitle;
    public readonly string Species = species;
    public readonly Gender Gender = gender;
    public readonly string? Fingerprint = fingerprint;
    public readonly string? Dna = dna;
    public readonly int? PagerNumber = pagerNumber;
}

[Serializable, NetSerializable]
public sealed class XoRecordsConsoleState(
    List<XoRecordListingEntry> listing,
    uint? selectedKey,
    bool selectedFlagged,
    XoRecordFields? published,
    bool isEditable,
    int discrepancyCount)
    : BoundUserInterfaceState
{
    public readonly List<XoRecordListingEntry> Listing = listing;
    public readonly uint? SelectedKey = selectedKey;
    public readonly bool SelectedFlagged = selectedFlagged;
    public readonly XoRecordFields? Published = published;
    public readonly bool IsEditable = isEditable;
    public readonly int DiscrepancyCount = discrepancyCount;
}

[Serializable, NetSerializable]
public sealed class XoSelectRecordMessage(uint? selectedKey) : BoundUserInterfaceMessage
{
    public readonly uint? SelectedKey = selectedKey;
}

[Serializable, NetSerializable]
public sealed class XoSubmitRecordMessage(uint id, XoRecordFields fields) : BoundUserInterfaceMessage
{
    public readonly uint Id = id;
    public readonly XoRecordFields Fields = fields;
}

[Serializable, NetSerializable]
public sealed class XoVerifyRecordMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class XoCreateRecordMessage : BoundUserInterfaceMessage
{
}

[Serializable, NetSerializable]
public sealed class XoDeleteRecordMessage(uint id) : BoundUserInterfaceMessage
{
    public readonly uint Id = id;
}
