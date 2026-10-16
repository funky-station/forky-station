using Robust.Shared.Player;

namespace Content.Shared.Chat;

public abstract partial class SharedChatSystem
{
    /// <summary>
    /// Dispatches an announcement to all.
    /// </summary>
    public virtual void DispatchGlobalAnnouncement(ChatAnnouncement chatAnnouncement)
    { }

    /// <summary>
    /// Dispatches an announcement to players selected by filter.
    /// </summary>
    /// <funky>
    /// If you have PA system exclusive announcements enabled, this doesn't differ from
    /// <see cref="DispatchGlobalAnnouncement"/> at all!
    /// </funky>
    public virtual void DispatchFilteredAnnouncement(ChatAnnouncement chatAnnouncement, Filter filter)
    { }

    /// <summary>
    /// Dispatches an announcement on a specific station.
    /// </summary>
    public virtual void DispatchStationAnnouncement(ChatAnnouncement chatAnnouncement, EntityUid? stationUid = null)
    { }
}
