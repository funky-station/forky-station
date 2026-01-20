// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 no
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;

namespace Content.Server.Ghost.Roles.UI
{
    public sealed class GhostRolesEui : BaseEui
    {
        private readonly GhostRoleSystem _ghostRoleSystem;

        public GhostRolesEui()
        {
            _ghostRoleSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<GhostRoleSystem>();
        }

        public override GhostRolesEuiState GetNewState()
        {
            return new(_ghostRoleSystem.GetGhostRolesInfo(Player));
        }

        public override void HandleMessage(EuiMessageBase msg)
        {
            base.HandleMessage(msg);

            switch (msg)
            {
                case RequestGhostRoleMessage req:
                    _ghostRoleSystem.Request(Player, req.Identifier);
                    break;
                case FollowGhostRoleMessage req:
                    _ghostRoleSystem.Follow(Player, req.Identifier);
                    break;
                case LeaveGhostRoleRaffleMessage req:
                    _ghostRoleSystem.LeaveRaffle(Player, req.Identifier);
                    break;
            }
        }

        public override void Closed()
        {
            base.Closed();

            _ghostRoleSystem.CloseEui(Player);
        }
    }
}
