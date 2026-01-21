// SPDX-FileCopyrightText: 2021, 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021, 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021-2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Paul <ritter.paul1+git@googlemail.com>
// SPDX-FileCopyrightText: 2022-2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 āda <ss.adasts@gmail.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Ertanic <36124833+Ertanic@users.noreply.github.com>
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
