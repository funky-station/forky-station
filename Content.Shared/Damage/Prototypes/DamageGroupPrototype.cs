// SPDX-FileCopyrightText: 2021 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 KrasnoshchekovPavel <119816022+KrasnoshchekovPavel@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
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
