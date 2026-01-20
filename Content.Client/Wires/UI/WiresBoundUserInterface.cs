// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;

namespace Content.Client.Wires.UI
{
    public sealed class WiresBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private WiresMenu? _menu;

        public WiresBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();
            _menu = this.CreateWindow<WiresMenu>();
            _menu.OnAction += PerformAction;
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            _menu?.Populate((WiresBoundUserInterfaceState) state);
        }

        public void PerformAction(int id, WiresAction action)
        {
            SendMessage(new WiresActionMessage(id, action));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;

            _menu?.Dispose();
        }
    }
}
