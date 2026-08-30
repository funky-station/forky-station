using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Unions;

[Serializable, NetSerializable]
public enum UnionClipboardUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class UnionClipboardNoteEntry(string title, string text, string author, string time)
{
    public string Title = title;
    public string Text = text;
    public string Author = author;
    public string Time = time;
}

[Serializable, NetSerializable]
public sealed class UnionClipboardMemberEntry(NetEntity entity, string name, string jobTitle, bool isLeader, List<UnionClipboardNoteEntry> notes)
{
    public NetEntity Entity = entity;
    public string Name = name;
    public string JobTitle = jobTitle;
    public bool IsLeader = isLeader;
    public List<UnionClipboardNoteEntry> Notes = notes;
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

[Serializable, NetSerializable]
public sealed class UnionClipboardAddNoteMessage(NetEntity target, string title, string text) : BoundUserInterfaceMessage
{
    public NetEntity Target = target;
    public string Title = title;
    public string Text = text;
}
