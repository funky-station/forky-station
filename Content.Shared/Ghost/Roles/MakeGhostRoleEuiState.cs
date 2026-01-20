// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles
{
    [Serializable, NetSerializable]
    public sealed class MakeGhostRoleEuiState : EuiStateBase
    {
        public MakeGhostRoleEuiState(NetEntity entity)
        {
            Entity = entity;
        }

        public NetEntity Entity { get; }
    }
}
