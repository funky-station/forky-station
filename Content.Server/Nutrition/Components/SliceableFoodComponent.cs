// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 FoLoKe
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Content.Server.Nutrition.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Nutrition.Components;

[RegisterComponent, Access(typeof(SliceableFoodSystem))]
public sealed partial class SliceableFoodComponent : Component
{
    /// <summary>
    /// Prototype to spawn after slicing.
    /// If null then it can't be sliced.
    /// </summary>
    [DataField]
    public EntProtoId? Slice;

    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Items/Culinary/chop.ogg");

    /// <summary>
    /// Number of slices the food starts with.
    /// </summary>
    [DataField("count")]
    public ushort TotalCount = 5;

    /// <summary>
    /// how long it takes for this food to be sliced
    /// </summary>
    [DataField]
    public float SliceTime = 1f;

    /// <summary>
    /// all the pieces will be shifted in random directions.
    /// </summary>
    [DataField]
    public float SpawnOffset = 0.5f;
}
