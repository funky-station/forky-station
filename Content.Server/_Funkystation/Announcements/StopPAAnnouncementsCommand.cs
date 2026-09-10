using Content.Server.Administration;
using Content.Shared._Funkystation.Communications;
using Content.Shared.Administration;
using Robust.Shared.Toolshed;

namespace Content.Server.Announcements;

[ToolshedCommand(Name = "stop_pa_announcements"), AdminCommand(AdminFlags.Admin)]
public sealed class StopPAAnnouncementsCommand : ToolshedCommand
{
    [CommandImplementation]
    public void StopAll()
    {
        var announcers = EntityManager.EntityQueryEnumerator<PAAnnouncerComponent>();
        while (announcers.MoveNext(out var comp))
        {
            comp.QueuedMessages.Clear();
        }
    }
}
