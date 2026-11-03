using Content.Shared._Funkystation.Pager.Components;
using Content.Shared.CartridgeLoader;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Pager;

[Serializable, NetSerializable]
public sealed class PagerCartridgeSendMessage(int targetNumber, string? code) : CartridgeMessageEvent
{
    public readonly int TargetNumber = targetNumber;
    public readonly string? Code = code;
}

[Serializable, NetSerializable]
public sealed class PagerCartridgeMassPageMessage(int prefix, string? code) : CartridgeMessageEvent
{
    public readonly int Prefix = prefix;
    public readonly string? Code = code;
}

[Serializable, NetSerializable]
public sealed class PagerCartridgeCycleModeMessage : CartridgeMessageEvent;

[Serializable, NetSerializable]
public sealed class PagerCartridgeUiState(int ownNumber, PagerMode mode, PagerLogEntry? currentPage, List<PagerDirectory> directories)
    : BoundUserInterfaceState
{
    public readonly int OwnNumber = ownNumber;
    public readonly PagerMode Mode = mode;
    public readonly PagerLogEntry? CurrentPage = currentPage;
    public readonly List<PagerDirectory> Directories = directories;
}

// one department the id can mass page, & its listed employees
[Serializable, NetSerializable]
public sealed record PagerDirectory(string DepartmentName, int Prefix, List<PagerDirectoryEntry> Entries);

[Serializable, NetSerializable]
public sealed record PagerDirectoryEntry(string Name, string JobTitle, int Number);
