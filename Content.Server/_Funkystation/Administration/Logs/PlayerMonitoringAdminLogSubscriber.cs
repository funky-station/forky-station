using Content.Server.Administration.Logs;
using Content.Server.Database;

namespace Content.Server._Funkystation.Administration.Logs;

/// <summary>
/// Extension point for post-queue admin log hooks; player monitoring no longer duplicates rows here.
/// </summary>
public sealed class PlayerMonitoringAdminLogSubscriber : IAdminLogQueuedSubscriber
{
    public void OnAdminLogQueued(AdminLog log)
    {
    }
}
