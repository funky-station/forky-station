using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class XoRecordsCVars
{
    /// <summary>
    /// Defines whether the XO personnel records console can manually edit crew manifest entries. If false, the published records automatically match the server's live records, like vanilla.
    /// </summary>
    public static readonly CVarDef<bool> ManualRecordsEnabled =
        CVarDef.Create("funkystation.xo_records.manual_enabled", true, CVar.SERVERONLY);
}
