// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Mind.Filters;

/// <summary>
/// A mind pool that uses <see cref="SharedMindSystem.AddAliveHumans"/>.
/// </summary>
public sealed partial class AliveHumansPool : IMindPool
{
    void IMindPool.FindMinds(HashSet<Entity<MindComponent>> minds, EntityUid? exclude, IEntityManager entMan, SharedMindSystem mindSys)
    {
        mindSys.AddAliveHumans(minds, exclude);
    }
}
