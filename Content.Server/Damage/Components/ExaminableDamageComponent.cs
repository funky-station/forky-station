// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Content.Shared.Dataset;
using Robust.Shared.Prototypes;

namespace Content.Server.Damage.Components;

/// <summary>
/// This component shows entity damage severity when it is examined by player.
/// </summary>
[RegisterComponent]
public sealed partial class ExaminableDamageComponent : Component
{
    /// <summary>
    /// ID of the <see cref="LocalizedDatasetPrototype"/> containing messages to display a different damage levels.
    /// The first message will be used at 0 damage with the others equally distributed across the range from undamaged to fully damaged.
    /// </summary>
    [DataField]
    public ProtoId<LocalizedDatasetPrototype>? Messages;
}
