using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class SistrCVars
{
    /// <summary>
    /// Whether or not a functional, undamaged SIS/TR core must be present on the station for station event announcements
    /// to be made.
    /// </summary>
    public static readonly CVarDef<bool> SistrRequired =
        CVarDef.Create("funkystation.sistr_required", true, CVar.SERVER | CVar.REPLICATED);
}
