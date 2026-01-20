// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 JustinTime
// SPDX-License-Identifier: MIT

using System.Threading;

namespace Content.Server.Resist;

[RegisterComponent]
[Access(typeof(ResistLockerSystem))]
public sealed partial class ResistLockerComponent : Component
{
    /// <summary>
    /// How long will this locker take to kick open, defaults to 2 minutes
    /// </summary>
    [DataField("resistTime")]
    public float ResistTime = 120f;

    /// <summary>
    /// For quick exit if the player attempts to move while already resisting
    /// </summary>
    [ViewVariables]
    public bool IsResisting = false;
}
