// SPDX-FileCopyrightText: 2024 Tornado Tech
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction.Steps
{
    public sealed partial class MultipleTagsConstructionGraphStep : ArbitraryInsertConstructionGraphStep
    {
        [DataField("allTags")]
        private List<ProtoId<TagPrototype>>? _allTags;

        [DataField("anyTags")]
        private List<ProtoId<TagPrototype>>? _anyTags;

        private static bool IsNullOrEmpty<T>(ICollection<T>? list)
        {
            return list == null || list.Count == 0;
        }

        public override bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory)
        {
            // This step can only happen if either list has tags.
            if (IsNullOrEmpty(_allTags) && IsNullOrEmpty(_anyTags))
                return false; // Step is somehow invalid, we return.

            var tagSystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();

            if (_allTags != null && !tagSystem.HasAllTags(uid, _allTags))
                return false; // We don't have all the tags needed.

            if (_anyTags != null && !tagSystem.HasAnyTag(uid, _anyTags))
                return false; // We don't have any of the tags needed.

            // This entity is valid!
            return true;
        }
    }
}
