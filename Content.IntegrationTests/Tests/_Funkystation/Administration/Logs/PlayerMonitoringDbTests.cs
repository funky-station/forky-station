using System.Text.Json;
using Content.Server.Database;

namespace Content.IntegrationTests.Tests._Funkystation.Administration.Logs;

[TestFixture]
public sealed class PlayerMonitoringDbTests
{
    [Test]
    public async Task ResolveUserIdByExactNameEmptyReturnsNull()
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings
        {
            Connected = false,
            DummyTicker = true,
        });
        var db = pair.Server.ResolveDependency<IServerDbManager>();
        var result = await db.ResolveUserIdByExactNameAsync("   ");
        Assert.That(result, Is.Null);
        await pair.CleanReturnAsync();
    }

    [Test]
    public async Task ResolveUserIdByExactNameUnknownReturnsNull()
    {
        // DummyTicker avoids RestartRound during pair init (SQLite is single-threaded in test mode).
        await using var pair = await PoolManager.GetServerClient(new PoolSettings
        {
            Connected = false,
            DummyTicker = true,
        });
        var db = pair.Server.ResolveDependency<IServerDbManager>();
        var result = await db.ResolveUserIdByExactNameAsync("___player_monitoring_no_such_user___");
        Assert.That(result, Is.Null);
        await pair.CleanReturnAsync();
    }

    [Test]
    public void MergeMonitoringDetailsCombinesKeys()
    {
        var existing = JsonDocument.Parse("""{"a":1,"job":"old"}""");
        var patch = JsonDocument.Parse("""{"disconnect_reason":"timeout","job":"new"}""");
        var merged = ServerDbBase.MergeMonitoringDetails(existing, patch);
        existing.Dispose();
        patch.Dispose();

        var root = merged.RootElement;
        Assert.That(root.GetProperty("a").GetInt32(), Is.EqualTo(1));
        Assert.That(root.GetProperty("job").GetString(), Is.EqualTo("new"));
        Assert.That(root.GetProperty("disconnect_reason").GetString(), Is.EqualTo("timeout"));
        merged.Dispose();
    }

    [Test]
    public void ComputeMaxIdleGapMinutes_UsesGapsBetweenStartEndAndLogs()
    {
        var start = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 1, 1, 13, 0, 0, DateTimeKind.Utc);
        var logs = new[]
        {
            new DateTime(2026, 1, 1, 12, 10, 0, DateTimeKind.Utc),
            new DateTime(2026, 1, 1, 12, 50, 0, DateTimeKind.Utc),
        };

        var max = ServerDbBase.ComputeMaxIdleGapMinutes(end, start, logs);
        Assert.That(max, Is.EqualTo(40).Within(0.02));
    }

    [Test]
    public void ComputeMaxIdleGapMinutes_EmptyLogsReturnsZero()
    {
        var end = new DateTime(2026, 1, 1, 13, 0, 0, DateTimeKind.Utc);
        var max = ServerDbBase.ComputeMaxIdleGapMinutes(end, null, Array.Empty<DateTime>());
        Assert.That(max, Is.EqualTo(0));
    }
}
