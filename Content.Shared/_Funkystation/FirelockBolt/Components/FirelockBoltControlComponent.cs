using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using System;

namespace Content.Shared._Funkystation.FirelockBolt.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FirelockBoltControlComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Override;

    [DataField]
    public TimeSpan ToggleDelay = TimeSpan.FromSeconds(0);

    [DataField, AutoNetworkedField]
    public bool AlarmActive;

    /// <summary>
    /// used to differentiate between player-closed and emergency-closed doors
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan LastManualInteractionTime;

    [DataField, AutoNetworkedField]
    public bool IsManualClose;

    /// <summary>
    /// Sound played when the manual override is engaged
    /// </summary>
    [DataField]
    public SoundSpecifier EnableSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/hydraulic-decompress.ogg");

    /// <summary>
    /// Sound played when the manual override is disengaged
    /// </summary>
    [DataField]
    public SoundSpecifier DisableSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/gears_clicking.ogg");
}
