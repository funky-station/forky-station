// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 Cojoke
// SPDX-License-Identifier: MIT

namespace Content.Shared.IgnitionSource;

/// <summary>
///     Raised in order to toggle the <see cref="IgnitionSourceComponent"/> on an entity on or off
/// </summary>
[ByRefEvent]
public readonly record struct IgnitionEvent(bool Ignite = false);
