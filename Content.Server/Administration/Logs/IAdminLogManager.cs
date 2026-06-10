using System.Text.Json;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Shared.Administration.Logs;

namespace Content.Server.Administration.Logs;

public interface IAdminLogManager : ISharedAdminLogManager
{
    void Initialize();
    Task Shutdown();
    void Update();

    void RoundStarting(int id);
    void RunLevelChanged(GameRunLevel level);

    // Funkystation - Admin Log Enhancement
    /// <summary>
    /// Persists queued in-round logs only; does not flush or drop the pre-round queue (safe during lobby transition).
    /// </summary>
    Task FlushInRoundAdminLogsAsync();

    /// <summary>
    /// Alias for <see cref="FlushInRoundAdminLogsAsync"/> for callers that run immediately before reading persisted admin logs.
    /// </summary>
    Task FlushPendingAdminLogsAsync() => FlushInRoundAdminLogsAsync();

    Task<List<SharedAdminLog>> All(LogFilter? filter = null, Func<List<SharedAdminLog>>? listProvider = null);
    IAsyncEnumerable<string> AllMessages(LogFilter? filter = null);
    IAsyncEnumerable<JsonDocument> AllJson(LogFilter? filter = null);
    Task<Round> Round(int roundId);
    Task<List<SharedAdminLog>> CurrentRoundLogs(LogFilter? filter = null);
    IAsyncEnumerable<string> CurrentRoundMessages(LogFilter? filter = null);
    IAsyncEnumerable<JsonDocument> CurrentRoundJson(LogFilter? filter = null);
    Task<Round> CurrentRound();
    Task<int> CountLogs(int round);
}
