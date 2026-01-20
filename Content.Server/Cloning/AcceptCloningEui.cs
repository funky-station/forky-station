// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.EUI;
using Content.Shared.Cloning;
using Content.Shared.Eui;
using Content.Shared.Mind;

namespace Content.Server.Cloning
{
    public sealed class AcceptCloningEui : BaseEui
    {
        private readonly EntityUid _mindId;
        private readonly MindComponent _mind;
        private readonly CloningPodSystem _cloningPodSystem;

        public AcceptCloningEui(EntityUid mindId, MindComponent mind, CloningPodSystem cloningPodSys)
        {
            _mindId = mindId;
            _mind = mind;
            _cloningPodSystem = cloningPodSys;
        }

        public override void HandleMessage(EuiMessageBase msg)
        {
            base.HandleMessage(msg);

            if (msg is not AcceptCloningChoiceMessage choice ||
                choice.Button == AcceptCloningUiButton.Deny)
            {
                Close();
                return;
            }

            _cloningPodSystem.TransferMindToClone(_mindId, _mind);
            Close();
        }
    }
}
