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

    public static readonly CVarDef<float> SupermatterSingulooseMolesModifier =
        CVarDef.Create("supermatter.singuloose_moles_modifier", 1f, CVar.SERVERONLY);

    /// <summary>
    /// Tile mole count above which singuloose can trigger (after modifiers).
    /// </summary>
    public static readonly CVarDef<float> SupermatterMolePenaltyThreshold =
        CVarDef.Create("supermatter.mole_penalty_threshold", 350f, CVar.SERVERONLY);

    public static readonly CVarDef<float> SupermatterPowerPenaltyThreshold =
        CVarDef.Create("supermatter.power_penalty_threshold", 5000f, CVar.SERVERONLY);

    public static readonly CVarDef<float> SupermatterTesloosePowerModifier =
        CVarDef.Create("supermatter.tesloose_power_modifier", 1f, CVar.SERVERONLY);

    /// <summary>
    ///  Resonance cascade when absorbed mix has high Anti-Nob + Hyper-Nob fraction and enough moles.
    /// </summary>
    public static readonly CVarDef<bool> SupermatterDoCascadeDelam =
        CVarDef.Create("supermatter.do_cascade_delam", true, CVar.SERVERONLY);

    /// <summary>
    /// Minimum total absorbed moles for cascade branch (wiki: above 270).
    /// </summary>
    public static readonly CVarDef<float> SupermatterCascadeMinAbsorbedMoles =
        CVarDef.Create("supermatter.cascade_min_absorbed_moles", 270f, CVar.SERVERONLY);

    /// <summary>
    /// Minimum mole fraction for Anti-Noblium and Hyper-Noblium each (wiki: more than 40%).
    /// </summary>
    public static readonly CVarDef<float> SupermatterCascadeNobMinFraction =
        CVarDef.Create("supermatter.cascade_nob_min_fraction", 0.4f, CVar.SERVERONLY);

    /// <summary>
    /// Singularity delam when absorbed moles exceed this at delamination time.
    /// </summary>
    public static readonly CVarDef<float> SupermatterSinguloAbsorbedMolesThreshold =
        CVarDef.Create("supermatter.singulo_absorbed_moles_threshold", 1800f, CVar.SERVERONLY);
}
