using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.StationTime.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StationTimeComponent : Component
{
    // server's DateTime.UtcNow.Ticks at the moment of sync
    [ViewVariables, AutoNetworkedField]
    public long RealUtcTicksAtSync;

    // shared simulation clock (IGameTiming.CurTime) at that same moment
    [ViewVariables, AutoNetworkedField]
    public TimeSpan CurTimeAtSync;
}
