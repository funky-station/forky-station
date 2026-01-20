// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 SlamBamActionman
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Chemistry.Reaction;

[RegisterComponent]
public sealed partial class ReactiveComponent : Component
{
    /// <summary>
    ///     A dictionary of reactive groups -> methods that work on them.
    /// </summary>
    [DataField("groups", readOnly: true, serverOnly: true,
        customTypeSerializer:
        typeof(PrototypeIdDictionarySerializer<HashSet<ReactionMethod>, ReactiveGroupPrototype>))]
    public Dictionary<string, HashSet<ReactionMethod>>? ReactiveGroups;

    /// <summary>
    ///     Special reactions that this prototype can specify, outside of any that reagents already apply.
    ///     Useful for things like monkey cubes, which have a really prototype-specific effect.
    /// </summary>
    [DataField("reactions", true, serverOnly: true)]
    public List<ReactiveReagentEffectEntry>? Reactions;
}

[DataDefinition]
public sealed partial class ReactiveReagentEffectEntry
{
    [DataField("methods")]
    public HashSet<ReactionMethod> Methods = default!;

    [DataField("reagents", customTypeSerializer: typeof(PrototypeIdHashSetSerializer<ReagentPrototype>))]
    public HashSet<string>? Reagents = null;

    [DataField("effects", required: true)]
    public EntityEffect[] Effects = default!;

    [DataField("groups", readOnly: true, serverOnly: true,
        customTypeSerializer:typeof(PrototypeIdDictionarySerializer<HashSet<ReactionMethod>, ReactiveGroupPrototype>))]
    public Dictionary<string, HashSet<ReactionMethod>>? ReactiveGroups { get; private set; }
}
