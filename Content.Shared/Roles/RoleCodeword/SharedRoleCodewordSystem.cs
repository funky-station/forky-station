// SPDX-FileCopyrightText: 2024 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Roles.RoleCodeword;

public abstract class SharedRoleCodewordSystem : EntitySystem
{
    public void SetRoleCodewords(Entity<RoleCodewordComponent> ent, string key, List<string> codewords, Color color)
    {
        var data = new CodewordsData(color, codewords);
        ent.Comp.RoleCodewords[key] = data;
        Dirty(ent);
    }
}
