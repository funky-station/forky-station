// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 JustinTime
// SPDX-License-Identifier: MIT

using Content.Shared.DoAfter;

namespace Content.Server.Resist;

[RegisterComponent]
public sealed partial class CanEscapeInventoryComponent : Component
{
    /// <summary>
    /// Base doafter length for uncontested breakouts.
    /// </summary>
    [DataField("baseResistTime")]
    public float BaseResistTime = 5f;

    public bool IsEscaping => DoAfter != null;

    [DataField("doAfter")]
    public DoAfterId? DoAfter;
}
