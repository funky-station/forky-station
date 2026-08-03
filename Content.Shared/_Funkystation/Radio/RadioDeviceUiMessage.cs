using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Radio;

[Serializable, NetSerializable]
public enum RadioVolumeUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class RadioVolumeSliderMessage(int value) : BoundUserInterfaceMessage
{
    public int Value = value;
}
