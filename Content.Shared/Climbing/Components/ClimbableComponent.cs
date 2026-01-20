// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 Ilya Chvilyov
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-License-Identifier: MIT

using Content.Shared.Interaction;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Climbing.Components
{
    /// <summary>
    /// Indicates this entity can be vaulted on top of.
    /// </summary>
    [RegisterComponent, NetworkedComponent]
    public sealed partial class ClimbableComponent : Component
    {
        /// <summary>
        ///     The range from which this entity can be climbed.
        /// </summary>
        [DataField] public float Range = SharedInteractionSystem.InteractionRange;

        /// <summary>
        /// Can drag-drop / verb vaulting be done? Set to false if climbing is being handled manually.
        /// </summary>
        [DataField]
        public bool Vaultable = true;

        /// <summary>
        ///     The time it takes to climb onto the entity.
        /// </summary>
        [DataField("delay")]
        public float ClimbDelay = 1.5f;

        /// <summary>
        ///     Sound to be played when a climb is started.
        /// </summary>
        [DataField("startClimbSound")]
        public SoundSpecifier? StartClimbSound = null;

        /// <summary>
        ///     Sound to be played when a climb finishes.
        /// </summary>
        [DataField("finishClimbSound")]
        public SoundSpecifier? FinishClimbSound = null;
    }
}
