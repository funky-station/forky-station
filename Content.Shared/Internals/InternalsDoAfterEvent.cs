// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Ciarán Walsh <github@ciaranwal.sh>
// SPDX-License-Identifier: MIT

using Content.Shared.Alert;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Internals;

public enum ToggleMode
{
    Toggle,
    On,
    Off
}

[Serializable, NetSerializable]
public sealed partial class InternalsDoAfterEvent : DoAfterEvent
{
    public ToggleMode ToggleMode = ToggleMode.Toggle;

    public InternalsDoAfterEvent(ToggleMode mode)
    {
        ToggleMode = mode;
    }

    public override DoAfterEvent Clone() => this;
}

public sealed partial class ToggleInternalsAlertEvent : BaseAlertEvent;
