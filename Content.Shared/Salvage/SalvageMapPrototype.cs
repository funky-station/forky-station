// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Salvage;

[Prototype]
public sealed partial class SalvageMapPrototype : IPrototype
{
    [ViewVariables] [IdDataField] public string ID { get; private set; } = default!;

    /// <summary>
    /// Relative directory path to the given map, i.e. `Maps/Salvage/template.yml`
    /// </summary>
    [DataField(required: true)] public ResPath MapPath;

    /// <summary>
    /// String that describes the size of the map.
    /// </summary>
    [DataField(required: true)]
    public LocId SizeString;
}
