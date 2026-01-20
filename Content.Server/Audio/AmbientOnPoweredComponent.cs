// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Audio;

namespace Content.Server.Audio
{
    /// <summary>
    /// Toggles <see cref="AmbientSoundComponent"/> on when powered and off when not powered.
    /// </summary>
    [RegisterComponent]
    public sealed partial class AmbientOnPoweredComponent : Component
    {
    }
}
