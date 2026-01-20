// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 Morb
// SPDX-License-Identifier: MIT

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Tags grid as nuke ops shuttle
/// </summary>
[RegisterComponent]
public sealed partial class NukeOpsShuttleComponent : Component
{
    [DataField]
    public EntityUid AssociatedRule;
}
