// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Milon
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Sound.Components;

/// <summary>
/// Simple sound emitter that emits sound on ActivateInWorld
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EmitSoundOnActivateComponent : BaseEmitSoundComponent
{
    /// <summary>
    ///     Whether or not to mark an interaction as handled after playing the sound. Useful if this component is
    ///     used to play sound for some other component with activation functionality.
    /// </summary>
    /// <remarks>
    ///     If false, you should be confident that the interaction will also be handled by some other system, as
    ///     otherwise this might enable sound spamming, as use-delays are only initiated if the interaction was
    ///     handled.
    /// </remarks>
    [DataField]
    public bool Handle = true;
}
