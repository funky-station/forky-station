using Robust.Shared.Audio;

namespace Content.Server._Funkystation.Communications;

[RegisterComponent]
public sealed partial class PAAnnouncerComponent : Component
{
    [DataField]
    public bool Enabled = true;

    [DataField]
    public bool PowerRequired = true;

    /// <summary>
    /// The queue of announcement messages to send.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public Queue<(string line, string author, Color? colorOverride, TimeSpan announceTime)> QueuedMessages = new();

    /// <summary>
    /// Used to enqueue announcement messages.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NextAnnounceTime = TimeSpan.Zero;

    /// <summary>
    /// A custom announcement sound this PA speaker
    /// should default to.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public SoundSpecifier? AnnouncementSound;

    /// <summary>
    /// Disables playing an announcement sound.
    /// </summary>
    [DataField]
    public bool Quiet = false;
}
