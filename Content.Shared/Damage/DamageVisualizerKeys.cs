// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Flipp Syder
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Damage
{
    [Serializable, NetSerializable]
    public enum DamageVisualizerKeys
    {
        Disabled,
        DamageSpecifierDelta,
        DamageUpdateGroups,
        ForceUpdate
    }

    [Serializable, NetSerializable]
    public sealed class DamageVisualizerGroupData : ICloneable
    {
        public List<string> GroupList;

        public DamageVisualizerGroupData(List<string> groupList)
        {
            GroupList = groupList;
        }

        public object Clone()
        {
            return new DamageVisualizerGroupData(new List<string>(GroupList));
        }
    }
}
