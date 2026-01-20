// SPDX-FileCopyrightText: 2024 Verm
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Jackw2As
// SPDX-License-Identifier: MIT

using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.Prototypes;

namespace Content.Server.Damage.Components;

[RegisterComponent]
public sealed partial class DamageOnToolInteractComponent : Component
{
    [DataField]
    public ProtoId<ToolQualityPrototype> Tools { get; private set; }

    // TODO: Remove this snowflake stuff, make damage per-tool quality perhaps?
    [DataField]
    public DamageSpecifier? WeldingDamage { get; private set; }

    [DataField]
    public DamageSpecifier? DefaultDamage { get; private set; }
}
