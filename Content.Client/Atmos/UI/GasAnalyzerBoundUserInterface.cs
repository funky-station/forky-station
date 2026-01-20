// SPDX-FileCopyrightText: 2025 themias
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 theashtronaut
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-License-Identifier: MIT

using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using static Content.Shared.Atmos.Components.GasAnalyzerComponent;

namespace Content.Client.Atmos.UI
{
    public sealed class GasAnalyzerBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private GasAnalyzerWindow? _window;

        public GasAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _window = this.CreateWindowCenteredLeft<GasAnalyzerWindow>();
            _window.OnClose += Close;
        }

        protected override void ReceiveMessage(BoundUserInterfaceMessage message)
        {
            if (_window == null)
                return;
            if (message is not GasAnalyzerUserMessage cast)
                return;
            _window.Populate(cast);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                _window?.Dispose();
        }
    }
}
