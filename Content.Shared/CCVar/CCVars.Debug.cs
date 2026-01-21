// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// Component to be inspected using the "Quick Inspect Component" keybind.
    /// Set by the "quickinspect" command.
    /// </summary>
    public static readonly CVarDef<string> DebugQuickInspect =
        CVarDef.Create("debug.quick_inspect", "", CVar.CLIENTONLY | CVar.ARCHIVE);
}
