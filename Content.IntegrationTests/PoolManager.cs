// SPDX-FileCopyrightText: 2022-2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022-2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022-2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Moony <moony@hellomouse.net>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Jake Huxell <JakeHuxell@pm.me>
// SPDX-License-Identifier: MIT

using System.Linq;
using System.Reflection;
using Content.IntegrationTests.Pair;
using Content.Shared.CCVar;
using Robust.UnitTesting;

namespace Content.IntegrationTests;

// The static class exist to avoid breaking changes
public static partial class PoolManager
{
    public static readonly ContentPoolManager Instance = new();
    public const string TestMap = "Empty";

    /// <summary>
    /// Runs a server, or a client until a condition is true
    /// </summary>
    /// <param name="instance">The server or client</param>
    /// <param name="func">The condition to check</param>
    /// <param name="maxTicks">How many ticks to try before giving up</param>
    /// <param name="tickStep">How many ticks to wait between checks</param>
    public static async Task WaitUntil(RobustIntegrationTest.IntegrationInstance instance, Func<bool> func,
        int maxTicks = 600,
        int tickStep = 1)
    {
        await WaitUntil(instance, async () => await Task.FromResult(func()), maxTicks, tickStep);
    }

    /// <summary>
    /// Runs a server, or a client until a condition is true
    /// </summary>
    /// <param name="instance">The server or client</param>
    /// <param name="func">The async condition to check</param>
    /// <param name="maxTicks">How many ticks to try before giving up</param>
    /// <param name="tickStep">How many ticks to wait between checks</param>
    public static async Task WaitUntil(RobustIntegrationTest.IntegrationInstance instance, Func<Task<bool>> func,
        int maxTicks = 600,
        int tickStep = 1)
    {
        var ticksAwaited = 0;
        bool passed;

        await instance.WaitIdleAsync();

        while (!(passed = await func()) && ticksAwaited < maxTicks)
        {
            var ticksToRun = tickStep;

            if (ticksAwaited + tickStep > maxTicks)
            {
                ticksToRun = maxTicks - ticksAwaited;
            }

            await instance.WaitRunTicks(ticksToRun);

            ticksAwaited += ticksToRun;
        }

        if (!passed)
        {
            Assert.Fail($"Condition did not pass after {maxTicks} ticks.\n" +
                        $"Tests ran ({instance.TestsRan.Count}):\n" +
                        $"{string.Join('\n', instance.TestsRan)}");
        }

        Assert.That(passed);
    }

    public static async Task<TestPair> GetServerClient(
        PoolSettings? settings = null,
        ITestContextLike? testContext = null)
    {
        return await Instance.GetPair(settings, testContext);
    }

    public static void Startup(params Assembly[] extra)
        => Instance.Startup(extra);

    public static void Shutdown() => Instance.Shutdown();
    public static string DeathReport() => Instance.DeathReport();
}

/// <summary>
/// Making clients, and servers is slow, this manages a pool of them so tests can reuse them.
/// </summary>
public sealed class ContentPoolManager : PoolManager<TestPair>
{
    public override PairSettings DefaultSettings =>  new PoolSettings();
    protected override string GetDefaultTestName(ITestContextLike testContext)
    {
        return testContext.FullName.Replace("Content.IntegrationTests.Tests.", "");
    }

    public override void Startup(params Assembly[] extraAssemblies)
    {
        DefaultCvars.AddRange(PoolManager.TestCvars);

        var shared = extraAssemblies
                .Append(typeof(Shared.Entry.EntryPoint).Assembly)
                .Append(typeof(PoolManager).Assembly)
                .ToArray();

        Startup([typeof(Client.Entry.EntryPoint).Assembly],
            [typeof(Server.Entry.EntryPoint).Assembly],
            shared);
    }
}
