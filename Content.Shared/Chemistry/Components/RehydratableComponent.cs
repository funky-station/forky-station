// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Arendian
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Chemistry.Components;

/// <summary>
/// Basically, monkey cubes.
/// But specifically, this component deletes the entity and spawns in a new entity when the entity is exposed to a certain amount of a given reagent.
/// </summary>
[RegisterComponent, Access(typeof(RehydratableSystem))]
public sealed partial class RehydratableComponent : Component
{
    /// <summary>
    /// The reagent that must be present to count as hydrated.
    /// </summary>
    [DataField("catalyst")]
    public ProtoId<ReagentPrototype> CatalystPrototype = "Water";

    /// <summary>
    /// The minimum amount of catalyst that must be present to be hydrated.
    /// </summary>
    [DataField]
    public FixedPoint2 CatalystMinimum = FixedPoint2.Zero;

    /// <summary>
    /// The entity to create when hydrated.
    /// </summary>
    [DataField(required: true)]
    public List<EntProtoId> PossibleSpawns = new();
}

/// <summary>
/// Raised on the rehydrated entity with target being the new entity it became.
/// </summary>
[ByRefEvent]
public readonly record struct GotRehydratedEvent(EntityUid Target);
