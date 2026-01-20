// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-License-Identifier: MIT

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// This is used for gamemodes that automatically respawn players when they're no longer alive.
/// </summary>
[RegisterComponent, Access(typeof(RespawnRuleSystem))]
public sealed partial class RespawnDeadRuleComponent : Component
{
    /// <summary>
    /// Whether or not we want to add everyone who dies to the respawn tracker
    /// </summary>
    [DataField]
    public bool AlwaysRespawnDead;
}
