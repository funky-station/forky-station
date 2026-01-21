// SPDX-FileCopyrightText: 2025 Red <96445749+TheShuEd@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.Bed.Sleep;

/// <summary>
/// Prevents waking up. Use only in conjunction with <see cref="StatusEffectComponent"/>, on the status effect entity.
/// </summary>
[NetworkedComponent, RegisterComponent]
public sealed partial class ForcedSleepingStatusEffectComponent : Component;
