using Content.Shared._Funkystation.Pager.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Pager;

[Serializable, NetSerializable]
public enum PagerUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class PagerSendPageMessage(int targetNumber, string? code) : BoundUserInterfaceMessage
{
    public readonly int TargetNumber = targetNumber;
    public readonly string? Code = code;
}

[Serializable, NetSerializable]
public sealed class PagerBoundUserInterfaceState(int ownNumber, PagerMode mode, PagerLogEntry? currentPage)
    : BoundUserInterfaceState
{
    public readonly int OwnNumber = ownNumber;
    public readonly PagerMode Mode = mode;
    public readonly PagerLogEntry? CurrentPage = currentPage;
}
