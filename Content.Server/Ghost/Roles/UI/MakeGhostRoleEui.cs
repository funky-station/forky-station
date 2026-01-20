// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;

namespace Content.Server.Ghost.Roles.UI
{
    public sealed class MakeGhostRoleEui : BaseEui
    {
        private IEntityManager _entManager;

        public MakeGhostRoleEui(IEntityManager entManager, NetEntity entity)
        {
            _entManager = entManager;
            Entity = entity;
        }

        public NetEntity Entity { get; }

        public override EuiStateBase GetNewState()
        {
            return new MakeGhostRoleEuiState(Entity);
        }

        public override void Closed()
        {
            base.Closed();

            _entManager.System<GhostRoleSystem>().CloseMakeGhostRoleEui(Player);
        }
    }
}
