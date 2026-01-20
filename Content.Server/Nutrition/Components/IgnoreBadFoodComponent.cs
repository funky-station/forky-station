// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using Content.Shared.Nutrition.EntitySystems;

namespace Content.Server.Nutrition.Components;

/// <summary>
/// This component allows NPC mobs to eat food with BadFoodComponent.
/// See MobMouseAdmeme for usage.
/// </summary>
[RegisterComponent]
public sealed partial class IgnoreBadFoodComponent : Component;
