// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Server.Power.EntitySystems;

namespace Content.Server.Power.Components
{
    [RegisterComponent]
    [Access(typeof(ExtensionCableSystem))]
    public sealed partial class ExtensionCableReceiverComponent : Component
    {
        [ViewVariables]
        public Entity<ExtensionCableProviderComponent>? Provider { get; set; }

        [ViewVariables]
        public bool Connectable = false;

        /// <summary>
        ///     The max distance from a <see cref="ExtensionCableProviderComponent"/> that this can receive power from.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("receptionRange")]
        public int ReceptionRange { get; set; } = 3;
    }
}
