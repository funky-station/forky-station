using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.HandheldNewsReader;

[Serializable, NetSerializable]
public sealed class HandheldNewsReaderUiMessage(HandheldNewsReaderUiAction action) : BoundUserInterfaceMessage
{
    public readonly HandheldNewsReaderUiAction Action = action;
}

[Serializable, NetSerializable]
public enum HandheldNewsReaderUiAction
{
    Next,
    Prev,
}
