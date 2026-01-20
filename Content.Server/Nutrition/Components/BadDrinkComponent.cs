// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using Content.Server.Nutrition.EntitySystems;

namespace Content.Server.Nutrition.Components;

/// <summary>
/// This component prevents NPC mobs like mice or cows from wanting to drink something that shouldn't be drank from.
/// Including but not limited to: puddles
/// </summary>
[RegisterComponent]
public sealed partial class BadDrinkComponent : Component;
