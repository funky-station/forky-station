// SPDX-FileCopyrightText: 2025 Winkarst-cpu
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 FoLoKe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-License-Identifier: MIT

using Content.Shared.Kitchen;
using Content.Shared.Storage;
using Robust.Shared.GameStates;

namespace Content.Shared.Nutrition.Components;

/// <summary>
/// Indicates that the entity can be butchered.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ButcherableComponent : Component
{
    /// <summary>
    /// List of the entities that this entity should spawn after being butchered.
    /// </summary>
    /// <remarks>
    /// Note that <see cref="SharedKitchenSpikeSystem"/> spawns one item at a time and decreases the amount until it's zero and then removes the entry.
    /// </remarks>
    [DataField("spawned", required: true), AutoNetworkedField]
    public List<EntitySpawnEntry> SpawnedEntities = [];

    /// <summary>
    /// Time required to butcher that entity.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float ButcherDelay = 8.0f;

    /// <summary>
    /// Tool type used to butcher that entity.
    /// </summary>
    [DataField("butcheringType"), AutoNetworkedField]
    public ButcheringType Type = ButcheringType.Knife;
}

public enum ButcheringType : byte
{
    /// <summary>
    /// E.g. goliaths.
    /// </summary>
    Knife,

    /// <summary>
    /// E.g. monkeys.
    /// </summary>
    Spike,

    /// <summary>
    /// E.g. humans.
    /// </summary>
    Gibber // TODO
}
