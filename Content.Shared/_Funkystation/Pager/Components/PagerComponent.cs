using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Pager.Components;

/// <summary>
/// marks entity as a pager
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedPagerSystem))]
public sealed partial class PagerComponent : Component
{
    /// <summary>
    /// the 4-digit number assigned to the pager
    /// </summary>
    [DataField, AutoNetworkedField]
    public int Number = -1;

    /// <summary>
    /// notification mode
    /// </summary>
    [DataField, AutoNetworkedField]
    public PagerMode Mode = PagerMode.Beep;

    /// <summary>
    /// timestamp for cooldown
    /// </summary>
    [DataField]
    public TimeSpan LastSent = TimeSpan.Zero;

    /// <summary>
    /// minimum time between pages
    /// </summary>
    [DataField]
    public TimeSpan SendCooldown = TimeSpan.FromSeconds(2);

    /// <summary>
    /// latest page received
    /// </summary>
    [DataField, AutoNetworkedField]
    public PagerLogEntry? CurrentPage;

    [DataField]
    public SoundSpecifier BeepSound = new SoundPathSpecifier("/Audio/Machines/twobeep.ogg", AudioParams.Default.WithMaxDistance(12f).WithVolume(3f));

    [DataField]
    public SoundSpecifier BuzzSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/Pager/pager-vibrate.ogg", AudioParams.Default.WithMaxDistance(8f).WithVolume(1f));

    /// <summary>
    /// whether this pager has been emagged
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Emagged;

    /// <summary>
    /// codes that cause the pager to violently explode when sent
    /// </summary>
    [DataField]
    public List<string> Blacklist = [];
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class PagerLogEntry
{
    [DataField]
    public int SenderNumber;

    [DataField]
    public string? Code;

    [DataField]
    public TimeSpan ReceivedAt;
}

public enum PagerMode : byte
{
    Beep,
    Buzz,
    Mute,
}
