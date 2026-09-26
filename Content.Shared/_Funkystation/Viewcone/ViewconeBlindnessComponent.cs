using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.Viewcone;

/// <summary>
/// Authoritative component used to reconcile client-only
/// fields found in <see cref="_ES.Viewcone.Components.ESViewconeComponent.cs"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class ViewconeBlindnessComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool IsBlind = false;

    [DataField, AutoNetworkedField]
    public ViewconeBlindnessReason Reason = ViewconeBlindnessReason.None;
}

[Flags]
public enum ViewconeBlindnessReason : byte
{
    None = 0,
    Storage = 1 << 0,
}

/// <summary>
///     Raised when a storage closes
///     Used by the client to predict its blidness state
/// </summary>
public readonly record struct ViewconeStorageClosedEvent();

/// <summary>
///     Raised when a storage opens
///     Used by the client to predict its blidness state
/// </summary>
public readonly record struct ViewconeStorageOpenedEvent();
