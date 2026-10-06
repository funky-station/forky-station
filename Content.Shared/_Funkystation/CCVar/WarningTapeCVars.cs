using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class WarningTapeCVars
{
    /// <summary>
    /// max length, in tiles, a single tape line can be stretched
    /// </summary>
    public static readonly CVarDef<int> MaxTiles =
        CVarDef.Create("funkystation.warning_tape.max_tiles", 6, CVar.SERVERONLY);
}
