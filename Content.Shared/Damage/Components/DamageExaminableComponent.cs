// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Damage.Components;

/// <summary>
/// Shows a detailed examine window with this entity's damage stats when examined.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DamageExaminableComponent : Component;
