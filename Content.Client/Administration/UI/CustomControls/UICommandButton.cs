// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Leo
// SPDX-License-Identifier: MIT

using System;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;

namespace Content.Client.Administration.UI.CustomControls
{
    public sealed class UICommandButton : CommandButton
    {
        public Type? WindowType { get; set; }
        private DefaultWindow? _window;

        protected override void Execute(ButtonEventArgs obj)
        {
            if (WindowType == null)
                return;
            _window = (DefaultWindow) IoCManager.Resolve<IDynamicTypeFactory>().CreateInstance(WindowType);
            _window?.OpenCentered();
        }
    }
}
