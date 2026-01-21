// SPDX-FileCopyrightText: 2022-2023 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Station;

/// <summary>
/// A config for a station. Specifies name and component modifications.
/// </summary>
[DataDefinition]
public sealed partial class StationConfig
{
    [DataField("stationProto", required: true)]
    public EntProtoId StationPrototype;

    [DataField("components", required: true)]
    public ComponentRegistry StationComponentOverrides = default!;
}

