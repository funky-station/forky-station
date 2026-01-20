// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Illiux
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Chemistry.Components;

/// <summary>
/// Allows the entity with this component to be placed in a <c>SharedReagentDispenserComponent</c>.
/// <para>Otherwise it's considered to be too large or the improper shape to fit.</para>
/// <para>Allows us to have obscenely large containers that are harder to abuse in chem dispensers
/// since they can't be placed directly in them.</para>
/// <see cref="Dispenser.SharedReagentDispenserComponent"/>
/// </summary>
[RegisterComponent]
[NetworkedComponent] // only needed for white-lists. Client doesn't actually need Solution data;
public sealed partial class FitsInDispenserComponent : Component
{
    /// <summary>
    /// Solution name that will interact with ReagentDispenserComponent.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Solution = "default";
}
