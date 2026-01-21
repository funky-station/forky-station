// SPDX-FileCopyrightText: 2023, 2025 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 āda <ss.adasts@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Nutrition.EntitySystems;

namespace Content.Server.Nutrition.Components;

/// <summary>
/// This component allows NPC mobs to eat food with BadFoodComponent.
/// See MobMouseAdmeme for usage.
/// </summary>
[RegisterComponent]
public sealed partial class IgnoreBadFoodComponent : Component;
