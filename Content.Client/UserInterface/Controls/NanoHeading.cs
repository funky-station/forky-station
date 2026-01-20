// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Client.Stylesheets;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls
{
    public sealed class NanoHeading : Container
    {
        private readonly Label _label;
        private readonly PanelContainer _panel;

        public NanoHeading()
        {
            _panel = new PanelContainer
            {
                Children = {(_label = new Label
                {
                    StyleClasses = {StyleClass.LabelHeading}
                })}
            };
            AddChild(_panel);

            HorizontalAlignment = HAlignment.Left;
        }

        public string? Text
        {
            get => _label.Text;
            set => _label.Text = value;
        }
    }
}
