using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Unions;

[Serializable, NetSerializable]
public enum UnionClipboardUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class UnionClipboardMemberEntry(NetEntity entity, string name, string jobTitle, bool isLeader)
{
    public NetEntity Entity = entity;
    public string Name = name;
    public string JobTitle = jobTitle;
    public bool IsLeader = isLeader;
}

[Serializable, NetSerializable]
public sealed class UnionClipboardBoundUserInterfaceState(string unionName, List<UnionClipboardMemberEntry> members) : BoundUserInterfaceState
{
    public string UnionName = unionName;
    public List<UnionClipboardMemberEntry> Members = members;
}

[Serializable, NetSerializable]
public sealed class UnionClipboardRemoveMemberMessage(NetEntity target) : BoundUserInterfaceMessage
{
    public NetEntity Target = target;
}
