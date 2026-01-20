// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Crotalus
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 0x6273
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Peter Wedder
// SPDX-License-Identifier: MIT

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Kitchen;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client.Kitchen.UI
{
    public sealed class ReagentGrinderBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private GrinderMenu? _menu;

        public ReagentGrinderBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _menu = this.CreateWindow<GrinderMenu>();
            _menu.OnToggleAuto += ToggleAutoMode;
            _menu.OnGrind += StartGrinding;
            _menu.OnJuice += StartJuicing;
            _menu.OnEjectAll += EjectAll;
            _menu.OnEjectBeaker += EjectBeaker;
            _menu.OnEjectChamber += EjectChamberContent;
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);
            if (state is not ReagentGrinderInterfaceState cState)
                return;

            _menu?.UpdateState(cState);
        }

        protected override void ReceiveMessage(BoundUserInterfaceMessage message)
        {
            base.ReceiveMessage(message);
            _menu?.HandleMessage(message);
        }

        public void ToggleAutoMode()
        {
            SendMessage(new ReagentGrinderToggleAutoModeMessage());
        }

        public void StartGrinding()
        {
            SendMessage(new ReagentGrinderStartMessage(GrinderProgram.Grind));
        }

        public void StartJuicing()
        {
            SendMessage(new ReagentGrinderStartMessage(GrinderProgram.Juice));
        }

        public void EjectAll()
        {
            SendMessage(new ReagentGrinderEjectChamberAllMessage());
        }

        public void EjectBeaker()
        {
            SendMessage(new ItemSlotButtonPressedEvent(SharedReagentGrinder.BeakerSlotId));
        }

        public void EjectChamberContent(EntityUid uid)
        {
            SendMessage(new ReagentGrinderEjectChamberContentMessage(EntMan.GetNetEntity(uid)));
        }
    }
}
