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
    public sealed partial class ExtensionCableProviderComponent : Component
    {
        /// <summary>
        ///     The max distance this can connect to <see cref="ExtensionCableReceiverComponent"/>s from.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("transferRange")]
        public int TransferRange { get; set; } = 3;

        [ViewVariables] public List<Entity<ExtensionCableReceiverComponent>> LinkedReceivers { get; } = new();

        /// <summary>
        ///     If <see cref="ExtensionCableReceiverComponent"/>s should consider connecting to this.
        /// </summary>
        [ViewVariables(VVAccess.ReadWrite)]
        public bool Connectable { get; set; } = true;


    }
}
