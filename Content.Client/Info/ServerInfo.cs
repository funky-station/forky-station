// SPDX-FileCopyrightText: 2026 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 Winkarst
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Jesse Rougeau
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-License-Identifier: MIT

using Content.Client.Changelog;
using Content.Client.Credits;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Client.Info
{
    public sealed class ServerInfo : BoxContainer
    {
        private readonly RichTextLabel _richTextLabel;

        public ServerInfo()
        {
            Orientation = LayoutOrientation.Vertical;

            _richTextLabel = new RichTextLabel
            {
                VerticalExpand = true
            };
            AddChild(_richTextLabel);
        }
        public void SetInfoBlob(string markup)
        {
            _richTextLabel.SetMessage(FormattedMessage.FromMarkupOrThrow(markup), tagsAllowed: null);
        }
    }
}
