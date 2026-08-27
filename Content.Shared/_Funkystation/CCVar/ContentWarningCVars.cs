using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class ContentWarningCVars
{
    public static readonly CVarDef<bool> Display =
        CVarDef.Create("content_warning.display", true, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<bool> KickOnIgnore =
        CVarDef.Create("content_warning.kick_on_ignore", true, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<bool> Acknowledged =
        CVarDef.Create("content_warning.acknowledged", false, CVar.CLIENTONLY | CVar.ARCHIVE);
}
