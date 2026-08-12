using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class RecognisableReagentCVars
{
    /// <summary>
    /// Whether recognisable reagents are shown when solutions are inspected
    /// </summary>
    public static readonly CVarDef<bool> RecognisableReagentsEnabled =
    CVarDef.Create("funkystation.recognisable_reagents_enabled", false, CVar.SERVER | CVar.REPLICATED);
}
