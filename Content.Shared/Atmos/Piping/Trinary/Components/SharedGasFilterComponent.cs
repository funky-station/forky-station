using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components;

[Serializable, NetSerializable]
public enum GasFilterUiKey
{
    Key,
}

[Serializable, NetSerializable]
public sealed class GasFilterToggleStatusMessage : BoundUserInterfaceMessage
{
    public bool Enabled { get; }

    public GasFilterToggleStatusMessage(bool enabled)
    {
        Enabled = enabled;
    }
}

[Serializable, NetSerializable]
public sealed class GasFilterChangeRateMessage : BoundUserInterfaceMessage
{
    public float Rate { get; }

    public GasFilterChangeRateMessage(float rate)
    {
        Rate = rate;
    }
}

[Serializable, NetSerializable]
public sealed class GasFilterSelectGasesMessage(List<Gas> gases) : BoundUserInterfaceMessage
{
    public readonly List<Gas> Gases = gases; // Funky - Allow multiple gases
}
