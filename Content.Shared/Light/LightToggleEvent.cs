// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Light;

public sealed class LightToggleEvent(bool isOn) : EntityEventArgs
{
    public bool IsOn = isOn;
}
