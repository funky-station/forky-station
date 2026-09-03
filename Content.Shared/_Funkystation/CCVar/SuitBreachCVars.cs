using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class SuitBreachCVars
{
    /// <summary>
    /// whether the suit breach system is enabled
    /// </summary>
    public static readonly CVarDef<bool> Enabled =
        CVarDef.Create("funkystation.suit_breach.enabled", true, CVar.SERVERONLY);

    /// <summary>
    /// damage threshold at or above which a breach is major instead of minor.
    /// </summary>
    public static readonly CVarDef<float> MajorDamageThreshold =
        CVarDef.Create("funkystation.suit_breach.major_damage_threshold", 25f, CVar.SERVERONLY);

    /// <summary>
    /// damage threshold at or above which a breach is catastrophic
    /// </summary>
    public static readonly CVarDef<float> CatastrophicDamageThreshold =
        CVarDef.Create("funkystation.suit_breach.catastrophic_damage_threshold", 60f, CVar.SERVERONLY);
}
