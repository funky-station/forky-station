// SPDX-FileCopyrightText: 2024 Debug
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-License-Identifier: MIT

using Content.Shared.Storage;

namespace Content.Server.Tools.Innate
{
    [RegisterComponent]
    public sealed partial class InnateToolComponent : Component
    {
        [DataField("tools")] public List<EntitySpawnEntry> Tools = new();
        public List<EntityUid> ToolUids = new();
        public List<string> ToSpawn = new();
    }
}
