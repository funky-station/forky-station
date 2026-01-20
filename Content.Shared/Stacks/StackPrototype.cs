// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Ertanic
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

namespace Content.Shared.Stacks;

/// <summary>
/// Prototype used to combine and spawn like-entities for <see cref="SharedStackSystem"/>.
/// </summary>
[Prototype]
public sealed partial class StackPrototype : IPrototype, IInheritingPrototype
{
    ///  <inheritdoc />
    [IdDataField]
    public string ID { get; private set; } = default!;

    ///  <inheritdoc />
    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<StackPrototype>))]
    public string[]? Parents { get; private set; }

    ///  <inheritdoc />
    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; private set; }

    /// <summary>
    /// Human-readable name for this stack type e.g. "Steel"
    /// </summary>
    /// <remarks>This is a localization string ID.</remarks>
    [DataField]
    public LocId Name { get; private set; } = string.Empty;

    /// <summary>
    /// An icon that will be used to represent this stack type.
    /// </summary>
    [DataField]
    public SpriteSpecifier? Icon { get; private set; }

    /// <summary>
    /// The entity id that will be spawned by default from this stack.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId<StackComponent> Spawn { get; private set; } = string.Empty;

    /// <summary>
    /// The maximum amount of things that can be in a stack, can be overriden on <see cref="StackComponent"/>.
    /// If null, simply has unlimited max count.
    /// </summary>
    [DataField]
    public int? MaxCount { get; private set; }
}
