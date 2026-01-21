// SPDX-FileCopyrightText: 2021-2022 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022-2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2024 ElectroJr <leonsfriedrich@gmail.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
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
