// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2020 Visne
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Peter Wedder
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client.Light.Components;

public sealed class HandheldLightStatus : Control
{
    private const float TimerCycle = 1;

    private readonly HandheldLightComponent _parent;
    private readonly PanelContainer[] _sections = new PanelContainer[HandheldLightComponent.StatusLevels - 1];

    private float _timer;

    private static readonly StyleBoxFlat StyleBoxLit = new()
    {
        BackgroundColor = Color.LimeGreen
    };

    private static readonly StyleBoxFlat StyleBoxUnlit = new()
    {
        BackgroundColor = Color.Black
    };

    public HandheldLightStatus(HandheldLightComponent parent)
    {
        _parent = parent;

        var wrapper = new BoxContainer
        {
            Orientation = LayoutOrientation.Horizontal,
            SeparationOverride = 4,
            HorizontalAlignment = HAlignment.Center
        };

        AddChild(wrapper);

        for (var i = 0; i < _sections.Length; i++)
        {
            var panel = new PanelContainer {MinSize = new Vector2(20, 20)};
            wrapper.AddChild(panel);
            _sections[i] = panel;
        }
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        _timer += args.DeltaSeconds;
        _timer %= TimerCycle;

        var level = _parent.Level;

        for (var i = 0; i < _sections.Length; i++)
        {
            if (i == 0)
            {
                if (level == 0 || level == null)
                {
                    _sections[0].PanelOverride = StyleBoxUnlit;
                }
                else if (level == 1)
                {
                    // Flash the last light.
                    _sections[0].PanelOverride = _timer > TimerCycle / 2 ? StyleBoxLit : StyleBoxUnlit;
                }
                else
                {
                    _sections[0].PanelOverride = StyleBoxLit;
                }

                continue;
            }

            _sections[i].PanelOverride = level >= i + 2 ? StyleBoxLit : StyleBoxUnlit;
        }
    }
}
