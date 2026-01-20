// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Server.Procedural;

/// <summary>
/// Added to pre-loaded maps for dungeon templates.
/// </summary>
[RegisterComponent]
public sealed partial class DungeonAtlasTemplateComponent : Component
{
    [DataField("path", required: true)]
    public ResPath Path;
}
