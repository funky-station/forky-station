// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

namespace Content.Server.Vocalization.Components;

/// <summary>
/// Used in combination with <see cref="VocalizerComponent"/>.
/// Blocks any attempts to vocalize if the entity has an <see cref="ApcPowerReceiverComponent"/>
/// and is currently unpowered.
/// </summary>
[RegisterComponent]
public sealed partial class VocalizerRequiresPowerComponent : Component;
