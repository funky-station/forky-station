// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Server.Antag.Components;

/// <summary>
/// Ghost role spawner that creates an antag for the associated gamerule.
/// </summary>
[RegisterComponent, Access(typeof(AntagSelectionSystem))]
public sealed partial class GhostRoleAntagSpawnerComponent : Component
{
    [DataField]
    public EntityUid? Rule;

    [DataField]
    public AntagSelectionDefinition? Definition;
}
