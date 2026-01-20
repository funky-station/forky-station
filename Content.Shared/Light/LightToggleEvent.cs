// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-License-Identifier: MIT

namespace Content.Shared.Light;

public sealed class LightToggleEvent(bool isOn) : EntityEventArgs
{
    public bool IsOn = isOn;
}
