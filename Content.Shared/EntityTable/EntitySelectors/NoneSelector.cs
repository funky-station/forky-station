// SPDX-FileCopyrightText: 2024-2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.EntityTable.EntitySelectors;

/// <summary>
/// Selects nothing.
/// </summary>
public sealed partial class NoneSelector : EntityTableSelector
{
    protected override IEnumerable<EntProtoId> GetSpawnsImplementation(System.Random rand,
        IEntityManager entMan,
        IPrototypeManager proto,
        EntityTableContext ctx)
    {
        yield break;
    }
}
