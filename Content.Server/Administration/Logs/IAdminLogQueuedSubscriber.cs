using Content.Server.Database;

namespace Content.Server.Administration.Logs;

public interface IAdminLogQueuedSubscriber
{
    void OnAdminLogQueued(AdminLog log);
}
