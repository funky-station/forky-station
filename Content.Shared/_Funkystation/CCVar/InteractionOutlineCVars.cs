using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class InteractionOutlineCVars
{
    // default colors based on colors from https://github.com/funky-station/forky-station/commit/cb9b846a8bc6849a04e54228007eca5ff11d1e13
    public static readonly CVarDef<string> ValidInteractionOutlineColor = CVarDef.Create(
        "funkystation.interaction_outline.in_range_color", "#d0ffea", CVar.CLIENTONLY | CVar.ARCHIVE);

    public static readonly CVarDef<string> InvalidInteractionOutlineColor = CVarDef.Create(
        "funkystation.interaction_outline.out_of_range_color", "#e64659", CVar.CLIENTONLY | CVar.ARCHIVE);
}
