// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-License-Identifier: MIT

using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Computer
{
    /// <summary>
    /// ComputerBoundUserInterface shunts all sorts of responsibilities that are in the BoundUserInterface for architectural reasons into the Window.
    /// NOTE: Despite the name, ComputerBoundUserInterface does not and will not care about things like power.
    /// </summary>
    [Virtual]
    public class ComputerBoundUserInterface<TWindow, TState> : ComputerBoundUserInterfaceBase where TWindow : BaseWindow, IComputerWindow<TState>, new() where TState : BoundUserInterfaceState
    {
        [ViewVariables]
        private TWindow? _window;

        protected override void Open()
        {
            base.Open();

            _window = this.CreateWindow<TWindow>();
            _window.SetupComputerWindow(this);
        }

        // Alas, this constructor has to be copied to the subclass. :(
        public ComputerBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            if (_window == null)
            {
                return;
            }

            _window.UpdateState((TState) state);
        }

        protected override void ReceiveMessage(BoundUserInterfaceMessage message)
        {
            _window?.ReceiveMessage(message);
        }
    }

    /// <summary>
    /// This class is to avoid a lot of &lt;&gt; being written when we just want to refer to SendMessage.
    /// We could instead qualify a lot of generics even further, but that is a waste of time.
    /// </summary>
    [Virtual]
    public class ComputerBoundUserInterfaceBase : BoundUserInterface
    {
        public ComputerBoundUserInterfaceBase(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        public new void SendMessage(BoundUserInterfaceMessage msg)
        {
            base.SendMessage(msg);
        }
    }

    public interface IComputerWindow<TState>
    {
        void SetupComputerWindow(ComputerBoundUserInterfaceBase cb)
        {
        }

        void UpdateState(TState state)
        {
        }

        void ReceiveMessage(BoundUserInterfaceMessage message)
        {
        }
    }
}

