// SPDX-FileCopyrightText: 2023 lzk
// SPDX-FileCopyrightText: 2023 Kevin Zheng
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-License-Identifier: MIT

using Content.Shared.Storage;
using Robust.Shared.Audio;

namespace Content.Server.Storage.Components
{
    /// <summary>
    ///     Spawns items when used in hand.
    /// </summary>
    [RegisterComponent]
    public sealed partial class SpawnItemsOnUseComponent : Component
    {
        /// <summary>
        ///     The list of entities to spawn, with amounts and orGroups.
        /// </summary>
        [DataField("items", required: true)]
        public List<EntitySpawnEntry> Items = new();

        /// <summary>
        ///     A sound to play when the items are spawned. For example, gift boxes being unwrapped.
        /// </summary>
        [DataField("sound")]
        public SoundSpecifier? Sound = null;

        /// <summary>
        ///     How many uses before the item should delete itself.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("uses")]
        public int Uses = 1;
    }
}
