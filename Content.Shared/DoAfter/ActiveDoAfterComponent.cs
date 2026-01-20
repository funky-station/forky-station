// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.DoAfter;

/// <summary>
///     Added to entities that are currently performing any doafters.
/// </summary>
[RegisterComponent]
public sealed partial class ActiveDoAfterComponent : Component
{
}
