using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class SmokingCancerCVars
{
    /// <summary>
    /// Whether or not smoking causes cancer.
    /// </summary>
    public static readonly CVarDef<bool> Cancer =
        CVarDef.Create("smoking_cancer.cancer", true, CVar.SERVER | CVar.REPLICATED);
}
