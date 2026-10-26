using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class DeathBlackoutCVars
{
    /// <summary>
    /// toggle, when off there is no blackout
    /// </summary>
    public static readonly CVarDef<bool> Enabled =
        CVarDef.Create("funkystation.death_blackout.enabled", true, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// whether hard crit darkens the screen and fades the audio towards death
    /// </summary>
    public static readonly CVarDef<bool> CritFade =
        CVarDef.Create("funkystation.death_blackout.crit_fade", true, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// seconds of black screen after death, ghosting is blocked for this long
    /// </summary>
    public static readonly CVarDef<float> Duration =
        CVarDef.Create("funkystation.death_blackout.duration", 10f, CVar.SERVER | CVar.REPLICATED);
}
