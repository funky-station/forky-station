// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Server.NPC.Queries.Curves;

[Prototype]
public sealed partial class UtilityCurvePresetPrototype : IPrototype
{
    [IdDataField] public string ID { get; private set; } = string.Empty;

    [DataField("curve", required: true)] public IUtilityCurve Curve = default!;
}
