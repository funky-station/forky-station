// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-License-Identifier: MIT

namespace Content.Server.Electrocution;

/// <summary>
/// Component for virtual electrocution entities (representing an in-progress shock).
/// </summary>
[RegisterComponent]
[Access(typeof(ElectrocutionSystem))]
public sealed partial class ElectrocutionComponent : Component
{
    [DataField("electrocuting")]
    public EntityUid Electrocuting;

    [DataField("source")]
    public EntityUid Source;

    [DataField("timeLeft")]
    public float TimeLeft;
}
