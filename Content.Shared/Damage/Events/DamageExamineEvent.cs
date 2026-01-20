// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-License-Identifier: MIT

using Content.Shared.Damage.Components;
using Robust.Shared.Utility;

namespace Content.Shared.Damage.Events;

/// <summary>
/// Raised on an entity with <see cref="DamageExaminableComponent"/> when examined to get the damage values displayed in the examine window.
/// </summary>
[ByRefEvent]
public readonly record struct DamageExamineEvent(FormattedMessage Message, EntityUid User);
