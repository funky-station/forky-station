// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Leo
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Server.Administration.UI
{
    [UsedImplicitly]
    public sealed class SetOutfitEui : BaseEui
    {
        [Dependency] private readonly IAdminManager _adminManager = default!;
        private readonly NetEntity _target;

        public SetOutfitEui(NetEntity entity)
        {
            _target = entity;
            IoCManager.InjectDependencies(this);
        }

        public override void Opened()
        {
            base.Opened();

            StateDirty();
            _adminManager.OnPermsChanged += AdminManagerOnPermsChanged;
        }

        public override EuiStateBase GetNewState()
        {
            return new SetOutfitEuiState
            {
                TargetNetEntity = _target,
            };
        }

        private void AdminManagerOnPermsChanged(AdminPermsChangedEventArgs obj)
        {
            // Close UI if user loses +FUN.
            if (obj.Player == Player && !UserAdminFlagCheck(AdminFlags.Fun))
            {
                Close();
            }
        }
        private bool UserAdminFlagCheck(AdminFlags flags)
        {
            return _adminManager.HasAdminFlag(Player, flags);
        }

    }
}
