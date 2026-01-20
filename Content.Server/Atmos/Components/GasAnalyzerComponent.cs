// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 theashtronaut
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 DmitriyRubetskoy
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-FileCopyrightText: 2020 derek
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Server.Atmos.Components;

/// <summary>
/// Used to keep track of which analyzers are active for update purposes
/// </summary>
[RegisterComponent]
public sealed partial class ActiveGasAnalyzerComponent : Component
{
    // Set to a tiny bit after the default because otherwise the user often gets a blank window when first using
    [DataField("accumulatedFrameTime"), ViewVariables(VVAccess.ReadWrite)]
    public float AccumulatedFrametime = 2.01f;

    /// <summary>
    /// How often to update the analyzer
    /// </summary>
    [DataField("updateInterval"), ViewVariables(VVAccess.ReadWrite)]
    public float UpdateInterval = 1f;
}
