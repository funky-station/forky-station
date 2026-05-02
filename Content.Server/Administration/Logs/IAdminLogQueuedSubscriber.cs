using Content.Server.Database;

namespace Content.Server.Administration.Logs;

// Funkystation - Admin Log Enhancement
public interface IAdminLogQueuedSubscriber
{
    void OnAdminLogQueued(AdminLog log);
}
