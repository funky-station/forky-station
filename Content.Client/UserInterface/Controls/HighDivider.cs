// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Client.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls
{
    public sealed class HighDivider : Control
    {
        public HighDivider()
        {
            Children.Add(new PanelContainer {StyleClasses = {StyleClass.HighDivider}});
        }
    }
}
