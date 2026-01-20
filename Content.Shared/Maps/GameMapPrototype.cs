// SPDX-FileCopyrightText: 2025 Errant
// SPDX-FileCopyrightText: 2025 Nemanja
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 ElectroJr
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2022 Mervill
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Diagnostics;
using Content.Shared.Station;

namespace Content.Shared.Maps;

/// <summary>
/// Prototype data for a game map.
/// </summary>
/// <remarks>
/// Forks should not directly edit existing parts of this class.
/// Make a new partial for your fancy new feature, it'll save you time later.
/// </remarks>
[Prototype, PublicAPI]
[DebuggerDisplay("GameMapPrototype [{ID} - {MapName}]")]
public sealed partial class GameMapPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public float MaxRandomOffset = 1000f;

    /// <summary>
    /// Turns out some of the map files are actually secretly grids. Excellent. I love map loading code.
    /// </summary>
    [DataField] public bool IsGrid;

    [DataField]
    public bool RandomRotation = true;

    /// <summary>
    /// Name of the map to use in generic messages, like the map vote.
    /// </summary>
    [DataField(required: true)]
    public string MapName { get; private set; } = default!;

    /// <summary>
    /// Relative directory path to the given map, i.e. `/Maps/saltern.yml`
    /// </summary>
    [DataField(required: true)]
    public ResPath MapPath { get; private set; } = default!;

    [DataField("stations", required: true)]
    private Dictionary<string, StationConfig> _stations = new();

    /// <summary>
    /// The stations this map contains. The names should match with the BecomesStation components.
    /// </summary>
    public IReadOnlyDictionary<string, StationConfig> Stations => _stations;

    /// <summary>
    /// Performs a shallow clone of this map prototype, replacing <c>MapPath</c> with the argument.
    /// </summary>
    public GameMapPrototype Persistence(ResPath mapPath)
    {
        return new()
        {
            ID = ID,
            MapName = MapName,
            MapPath = mapPath,
            _stations = _stations
        };
    }
}
