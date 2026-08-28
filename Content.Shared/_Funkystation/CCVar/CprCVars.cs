using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class CprCVars
{
    /// <summary>
    /// when enabled, CPR auto-repeats while target is still crit. otherwise you have to do each cpr pump manually
    /// </summary>
    public static readonly CVarDef<bool> Repeat =
        CVarDef.Create("funkystation.cpr.repeat", true, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// chance for a CPR pump to revive
    /// </summary>
    public static readonly CVarDef<float> ReviveChance =
        CVarDef.Create("funkystation.cpr.revive_chance", 0.05f, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// asphyxiation damage healed per CPR pump
    /// </summary>
    public static readonly CVarDef<float> AirlossHealAmount =
        CVarDef.Create("funkystation.cpr.airloss_heal_amount", 0f, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// delay in seconds for the CPR doafter
    /// </summary>
    public static readonly CVarDef<float> DoAfterDelay =
        CVarDef.Create("funkystation.cpr.do_after_delay", 0.5f, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// number of CPR pumps before ribs crack
    /// </summary>
    public static readonly CVarDef<int> RibCrackPump =
        CVarDef.Create("funkystation.cpr.rib_crack_pump", 6, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// blunt damage dealt when a patient's ribs crack from CPR
    /// </summary>
    public static readonly CVarDef<float> RibCrackDamage =
        CVarDef.Create("funkystation.cpr.rib_crack_damage", 35f, CVar.SERVER | CVar.REPLICATED);
}
