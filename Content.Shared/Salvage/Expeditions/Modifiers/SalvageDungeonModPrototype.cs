// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 chavonadelal
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using Content.Shared.Procedural;
using Robust.Shared.Prototypes;

namespace Content.Shared.Salvage.Expeditions.Modifiers;

[Prototype]
public sealed partial class SalvageDungeonModPrototype : IPrototype, IBiomeSpecificMod
{
    [IdDataField] public string ID { get; private set; } = default!;

    [DataField("desc")] public LocId Description { get; private set; } = string.Empty;

    /// <inheridoc/>
    [DataField("cost")]
    public float Cost { get; private set; } = 0f;

    /// <inheridoc/>
    [DataField]
    public List<ProtoId<SalvageBiomeModPrototype>>? Biomes { get; private set; } = null;

    /// <summary>
    /// The config to use for spawning the dungeon.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<DungeonConfigPrototype> Proto = string.Empty;
}
