// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Chris V
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Materials;
/// <summary>
/// Empty component that marks an entity as a "raw" material.
/// The material amounts themselves are in <see cref="PhysicalCompositionComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class MaterialComponent : Component
{

}

