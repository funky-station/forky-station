// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 20kdc
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Gravity
{
    [RegisterComponent]
    [AutoGenerateComponentState]
    [NetworkedComponent]
    public sealed partial class GravityComponent : Component
    {
        [DataField, AutoNetworkedField]
        public SoundSpecifier GravityShakeSound { get; set; } = new SoundPathSpecifier("/Audio/Effects/alert.ogg");

        [DataField, AutoNetworkedField]
        public bool Enabled;

        /// <summary>
        /// Inherent gravity ensures GravitySystem won't change Enabled according to the gravity generators attached to this entity.
        /// </summary>
        [DataField, AutoNetworkedField]
        public bool Inherent;
    }
}
