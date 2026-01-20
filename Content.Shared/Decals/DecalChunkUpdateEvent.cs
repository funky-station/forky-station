// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;
using static Content.Shared.Decals.DecalGridComponent;

namespace Content.Shared.Decals
{
    [Serializable, NetSerializable]
    public sealed class DecalChunkUpdateEvent : EntityEventArgs
    {
        public Dictionary<NetEntity, Dictionary<Vector2i, DecalChunk>> Data = new();
        public Dictionary<NetEntity, HashSet<Vector2i>> RemovedChunks = new();
    }
}
