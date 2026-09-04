using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class DefibrillatorCVars
{
    /// <summary>
    /// chance for a zap to resuscitate a dead person
    /// </summary>
    public static readonly CVarDef<float> ReviveChance =
        CVarDef.Create("funkystation.defib.revive_chance", 0.4f, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// amount of adrenaline reagent consumed per zap
    /// </summary>
    public static readonly CVarDef<float> AdrenalineCost =
        CVarDef.Create("funkystation.defib.adrenaline_cost", 5f, CVar.SERVER | CVar.REPLICATED);
}
