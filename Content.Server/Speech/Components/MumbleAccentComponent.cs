// SPDX-FileCopyrightText: 2025 themias
// SPDX-FileCopyrightText: 2023 dahnte
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Pancake
// SPDX-FileCopyrightText: 2022 Pancake
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;

namespace Content.Server.Speech.Components;

[RegisterComponent]
public sealed partial class MumbleAccentComponent : Component
{
    /// <summary>
    /// This modifies the audio parameters of emote sounds, screaming, laughing, etc.
    /// By default, it reduces the volume and distance of emote sounds.
    /// </summary>
    [DataField]
    public AudioParams EmoteAudioParams = AudioParams.Default.WithVolume(-8f).WithMaxDistance(5);
}
