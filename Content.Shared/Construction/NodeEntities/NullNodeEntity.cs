// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;

namespace Content.Shared.Construction.NodeEntities;

[UsedImplicitly]
[DataDefinition]
public sealed partial class NullNodeEntity : IGraphNodeEntity
{
    public string? GetId(EntityUid? uid, EntityUid? userUid, GraphNodeEntityArgs args)
    {
        return null;
    }
}
