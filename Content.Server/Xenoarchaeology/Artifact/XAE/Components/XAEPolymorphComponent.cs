// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2023 brainfood1183
// SPDX-License-Identifier: MIT

using Content.Shared.Polymorph;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Xenoarchaeology.Artifact.XAE.Components;

/// <summary>
/// Artifact polymorphs entities when triggered.
/// </summary>
[RegisterComponent, Access(typeof(XAEPolymorphSystem))]
public sealed partial class XAEPolymorphComponent : Component
{
    /// <summary>
    /// The polymorph effect to trigger.
    /// </summary>
    [DataField]
    public ProtoId<PolymorphPrototype> PolymorphPrototypeName = "ArtifactMonkey";

    /// <summary>
    /// Range of the effect.
    /// </summary>
    [DataField]
    public float Range = 2f;

    /// <summary>
    /// Sound to play on polymorph.
    /// </summary>
    [DataField]
    public SoundSpecifier PolySound = new SoundPathSpecifier("/Audio/Weapons/Guns/Gunshots/Magic/staff_animation.ogg");
}
