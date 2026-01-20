// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Damage
{
    /// <summary>
    ///     A set of coefficients or flat modifiers to damage types. Can be applied to <see cref="DamageSpecifier"/> using <see
    ///     cref="DamageSpecifier.ApplyModifierSet(DamageSpecifier, DamageModifierSet)"/>. This can be done several times as the
    ///     <see cref="DamageSpecifier"/> is passed to it's final target. By default the receiving <see cref="DamageableComponent"/>, will
    ///     also apply it's own <see cref="DamageModifierSet"/>.
    /// </summary>
    /// <remarks>
    /// The modifier will only ever be applied to damage that is being dealt. Healing is unmodified.
    /// </remarks>
    [DataDefinition]
    [Serializable, NetSerializable]
    [Virtual]
    public partial class DamageModifierSet
    {
        [DataField("coefficients", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
        public Dictionary<string, float> Coefficients = new();

        [DataField("flatReductions", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
        public Dictionary<string, float> FlatReduction = new();
    }
}
