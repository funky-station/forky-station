// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;

namespace Content.Shared.Movement.Events;

/// <summary>
/// Raised on a jetpack whenever it is toggled.
/// </summary>
public sealed partial class ToggleJetpackEvent : InstantActionEvent {}
