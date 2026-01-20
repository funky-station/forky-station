// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 KrasnoshchekovPavel
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Damage.Prototypes
{
    /// <summary>
    ///     A Group of <see cref="DamageTypePrototype"/>s.
    /// </summary>
    /// <remarks>
    ///     These groups can be used to specify supported damage types of a <see cref="DamageContainerPrototype"/>, or
    ///     to change/get/set damage in a <see cref="DamageableComponent"/>.
    /// </remarks>
    [Prototype(2)]
    public sealed partial class DamageGroupPrototype : IPrototype
    {
        [IdDataField] public string ID { get; private set; } = default!;

        [DataField(required: true)]
        private LocId Name { get; set; }

        [ViewVariables(VVAccess.ReadOnly)]
        public string LocalizedName => Loc.GetString(Name);

        [DataField(required: true)]
        public List<ProtoId<DamageTypePrototype>> DamageTypes { get; private set; } = default!;
    }
}
