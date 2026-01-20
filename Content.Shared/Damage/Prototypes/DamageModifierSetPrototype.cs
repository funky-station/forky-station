// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Damage.Prototypes
{
    /// <summary>
    ///     A version of DamageModifierSet that can be serialized as a prototype, but is functionally identical.
    /// </summary>
    /// <remarks>
    ///     Done to avoid removing the 'required' tag on the ID and passing around a 'prototype' when we really
    ///     just want normal data to be deserialized.
    /// </remarks>
    [Prototype]
    public sealed partial class DamageModifierSetPrototype : DamageModifierSet, IPrototype
    {
        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;
    }
}
