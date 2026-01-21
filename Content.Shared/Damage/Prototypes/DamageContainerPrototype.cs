// SPDX-FileCopyrightText: 2021 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Damage.Prototypes
{
    /// <summary>
    ///     A damage container which can be used to specify support for various damage types.
    /// </summary>
    /// <remarks>
    ///     This is effectively just a list of damage types that can be specified in YAML files using both damage types
    ///     and damage groups. Currently this is only used to specify what damage types a <see
    ///     cref="DamageableComponent"/> should support.
    /// </remarks>
    [Prototype]
    public sealed partial class DamageContainerPrototype : IPrototype
    {
        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;

        /// <summary>
        ///     List of damage groups that are supported by this container.
        /// </summary>
        [DataField]
        public List<ProtoId<DamageGroupPrototype>> SupportedGroups = new();

        /// <summary>
        ///     Partial List of damage types supported by this container. Note that members of the damage groups listed
        ///     in <see cref="SupportedGroups"/> are also supported, but they are not included in this list.
        /// </summary>
        [DataField]
        public List<ProtoId<DamageTypePrototype>> SupportedTypes = new();
    }
}
