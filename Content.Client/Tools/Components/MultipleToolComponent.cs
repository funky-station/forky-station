// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Client.Message;
using Content.Client.Stylesheets;
using Content.Shared.Tools.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client.Tools.Components
{
    public sealed class MultipleToolStatusControl : Control
    {
        private readonly MultipleToolComponent _parent;
        private readonly RichTextLabel _label;

        public MultipleToolStatusControl(MultipleToolComponent parent)
        {
            _parent = parent;
            _label = new RichTextLabel { StyleClasses = { StyleClass.ItemStatus } };
            _label.SetMarkup(_parent.StatusShowBehavior ? _parent.CurrentQualityName : string.Empty);
            AddChild(_label);
        }

        protected override void FrameUpdate(FrameEventArgs args)
        {
            base.FrameUpdate(args);

            if (_parent.UiUpdateNeeded)
            {
                _parent.UiUpdateNeeded = false;
                Update();
            }
        }

        public void Update()
        {
            _label.SetMarkup(_parent.StatusShowBehavior ? _parent.CurrentQualityName : string.Empty);
        }
    }
}
