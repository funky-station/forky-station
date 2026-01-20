// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Milon
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;

namespace Content.Shared.Sound.Components;

/// <summary>
/// Base sound emitter which defines most of the data fields.
/// Accepts both single sounds and sound collections.
/// </summary>
public abstract partial class BaseEmitSoundComponent : Component
{
    /// <summary>
    /// The <see cref="SoundSpecifier"/> to play.
    /// </summary>
    [DataField(required: true), AutoNetworkedField]
    public SoundSpecifier? Sound;

    /// <summary>
    /// Play the sound at the position instead of parented to the source entity.
    /// Useful if the entity is deleted after.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Positional;
}
