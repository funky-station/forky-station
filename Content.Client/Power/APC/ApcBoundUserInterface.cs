// SPDX-FileCopyrightText: 2024 eoineoineoin
// SPDX-FileCopyrightText: 2023 James Simonson
// SPDX-FileCopyrightText: 2024 James Simonson
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2020 Visne
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ShadowCommander
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2019 moneyl
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Client.Power.APC.UI;
using Content.Shared.Access.Systems;
using Content.Shared.APC;
using JetBrains.Annotations;
using Robust.Client.UserInterface;
using Robust.Shared.Player;

namespace Content.Client.Power.APC
{
    [UsedImplicitly]
    public sealed class ApcBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private ApcMenu? _menu;

        public ApcBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();
            _menu = this.CreateWindow<ApcMenu>();
            _menu.SetEntity(Owner);
            _menu.OnBreaker += BreakerPressed;

            var hasAccess = false;
            if (PlayerManager.LocalEntity != null)
            {
                var accessReader = EntMan.System<AccessReaderSystem>();
                hasAccess = accessReader.IsAllowed((EntityUid)PlayerManager.LocalEntity, Owner);
            }
            _menu?.SetAccessEnabled(hasAccess);
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            var castState = (ApcBoundInterfaceState) state;
            _menu?.UpdateState(castState);
        }

        public void BreakerPressed()
        {
            SendMessage(new ApcToggleMainBreakerMessage());
        }
    }
}
