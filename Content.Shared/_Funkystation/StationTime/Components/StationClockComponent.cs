using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Funkystation.StationTime.Components;

/// <summary>
/// component for entities that tell time on examine
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StationClockComponent : Component
{
    [DataField]
    public bool ShowSeconds;

    [DataField]
    public bool ShowDate = true;

    [DataField]
    public bool ShowTemperature;

    // how often to check atmos
    [DataField]
    public float TemperatureUpdateInterval = 5f;

    [ViewVariables, AutoNetworkedField]
    public float? LastTemperatureCelsius;

    [ViewVariables]
    public float Accumulator;

    [DataField]
    public List<string> ScreenStates = [];

    // whether the clock is currently switched on
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    // played whenever the clock is toggled on or off
    [DataField]
    public SoundSpecifier? ToggleSound;
}
