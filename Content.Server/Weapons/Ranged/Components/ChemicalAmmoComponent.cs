// SPDX-FileCopyrightText: 2026 mq
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

namespace Content.Server.Weapons.Ranged.Components;

[RegisterComponent]
public sealed partial class ChemicalAmmoComponent : Component
{
    public const string DefaultSolutionName = "ammo";

    [DataField("solution")]
    public string SolutionName { get; set; } = DefaultSolutionName;
}
