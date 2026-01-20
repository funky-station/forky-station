// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Stunnable;

/// <summary>
/// This is used to temporarily prevent an entity from moving or acting.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedStunSystem))]
public sealed partial class StunnedComponent : Component;
