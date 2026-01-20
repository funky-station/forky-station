// SPDX-FileCopyrightText: 2025 Brandon Li
// SPDX-FileCopyrightText: 2025 Red
// SPDX-FileCopyrightText: 2024 chavonadelal
// SPDX-FileCopyrightText: 2024 Winkarst
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-License-Identifier: MIT

using Content.Client.Stylesheets;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client.Actions.UI
{
    /// <summary>
    /// Tooltip for actions or alerts because they are very similar.
    /// </summary>
    public sealed class ActionAlertTooltip : PanelContainer
    {
        private const float TooltipTextMaxWidth = 350;

        private readonly RichTextLabel _cooldownLabel;
        private readonly IGameTiming _gameTiming;

        /// <summary>
        /// Current cooldown displayed in this tooltip. Set to null to show no cooldown.
        /// </summary>
        public (TimeSpan Start, TimeSpan End)? Cooldown { get; set; }

        public ActionAlertTooltip(FormattedMessage name, FormattedMessage? desc, string? requires = null)
        {
            Stylesheet = IoCManager.Resolve<IStylesheetManager>().SheetSystem;
            _gameTiming = IoCManager.Resolve<IGameTiming>();

            SetOnlyStyleClass(StyleClass.TooltipPanel);

            BoxContainer vbox;
            AddChild(vbox = new BoxContainer
            {
                Orientation = LayoutOrientation.Vertical,
                RectClipContent = true
            });
            var nameLabel = new RichTextLabel
            {
                MaxWidth = TooltipTextMaxWidth,
                StyleClasses = { StyleClass.TooltipTitle }
            };
            nameLabel.SetMessage(name);
            vbox.AddChild(nameLabel);

            if (desc != null && !string.IsNullOrWhiteSpace(desc.ToString()))
            {
                var description = new RichTextLabel
                {
                    MaxWidth = TooltipTextMaxWidth,
                    StyleClasses = { StyleClass.TooltipDesc }
                };
                description.SetMessage(desc);
                vbox.AddChild(description);
            }

            vbox.AddChild(_cooldownLabel = new RichTextLabel
            {
                MaxWidth = TooltipTextMaxWidth,
                StyleClasses = { StyleClass.TooltipDesc },
                Visible = false
            });

            if (!string.IsNullOrWhiteSpace(requires))
            {
                var requiresLabel = new RichTextLabel
                {
                    MaxWidth = TooltipTextMaxWidth,
                    StyleClasses = { StyleClass.TooltipDesc }
                };

                if (!FormattedMessage.TryFromMarkup("[color=#635c5c]" + requires + "[/color]", out var markup))
                    return;

                requiresLabel.SetMessage(markup);

                vbox.AddChild(requiresLabel);
            }
        }

        protected override void FrameUpdate(FrameEventArgs args)
        {
            base.FrameUpdate(args);
            if (!Cooldown.HasValue)
            {
                _cooldownLabel.Visible = false;
                return;
            }

            var timeLeft = Cooldown.Value.End - _gameTiming.CurTime;
            if (timeLeft > TimeSpan.Zero)
            {
                var duration = Cooldown.Value.End - Cooldown.Value.Start;

                if (!FormattedMessage.TryFromMarkup(Loc.GetString("ui-actionslot-duration", ("duration", (int)duration.TotalSeconds), ("timeLeft", (int)timeLeft.TotalSeconds + 1)), out var markup))
                    return;

                _cooldownLabel.SetMessage(markup);
                _cooldownLabel.Visible = true;
            }
            else
            {
                _cooldownLabel.Visible = false;
            }
        }
    }
}
