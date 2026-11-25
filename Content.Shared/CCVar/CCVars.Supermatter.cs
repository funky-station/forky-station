using Content.Shared._Funkystation.SM;
using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    /// When true, <see cref="SupermatterForcedDelamType"/> is used instead of normal delam branching.
    /// </summary>
    public static readonly CVarDef<bool> SupermatterDoForceDelam =
        CVarDef.Create("supermatter.do_force_delam", false, CVar.SERVERONLY);

    /// <summary>
    /// Forced delamination outcome when <see cref="SupermatterDoForceDelam"/> is enabled.
    /// </summary>
    public static readonly CVarDef<int> SupermatterForcedDelamType =
        CVarDef.Create("supermatter.forced_delam_type", (int)DelamType.Explosion, CVar.SERVERONLY);

    public static readonly CVarDef<bool> SupermatterDoSingulooseDelam =
        CVarDef.Create("supermatter.do_singuloose_delam", true, CVar.SERVERONLY);

    public static readonly CVarDef<bool> SupermatterDoTeslooseDelam =
        CVarDef.Create("supermatter.do_tesloose_delam", true, CVar.SERVERONLY);


    /// <summary>
    ///  Resonance cascade when Negative Enthalpy, or Positive Stability or two characteristics tie.
    /// </summary>
    public static readonly CVarDef<bool> SupermatterDoCascadeDelam =
        CVarDef.Create("supermatter.do_cascade_delam", true, CVar.SERVERONLY);

    public static readonly CVarDef<bool> SupermatterDoAlerts =
        CVarDef.Create("supermatter.do_alerts", true, CVar.SERVERONLY);

}
