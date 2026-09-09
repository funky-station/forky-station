using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.FirelockBolt;

[Serializable, NetSerializable]
public enum FirelockOverrideUiKey : byte
{
    Key
}

/// <summary>
/// Sent once the lever has been dragged to one end and released
/// </summary>
[Serializable, NetSerializable]
public sealed class FirelockOverrideSetMessage(bool @override) : BoundUserInterfaceMessage
{
    public readonly bool Override = @override;
}

/// <summary>
/// Pushed to the lever UI whenever the override or bolt state changes
/// </summary>
[Serializable, NetSerializable]
public sealed class FirelockOverrideBuiState(bool @override, bool bolted) : BoundUserInterfaceState
{
    public readonly bool Override = @override;
    public readonly bool Bolted = bolted;
}
