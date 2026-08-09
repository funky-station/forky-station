namespace Content.Server._Funkystation.Communications;

[RegisterComponent]
public sealed partial class PAAnnouncerComponent : Component
{
    /// <summary>
    /// The queue of announcement messages to send.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public Queue<(string line, string author, TimeSpan announceTime)> QueuedMessages = new();

    /// <summary>
    /// Used to enqueue announcement messages.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan NextAnnounceTime = TimeSpan.Zero;
}
