// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.Hands.UI
{
    public sealed class HandVirtualItemStatus : Control
    {
        public HandVirtualItemStatus()
        {
            RobustXamlLoader.Load(this);
        }
    }
}
