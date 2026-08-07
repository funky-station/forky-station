using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class ChatPrefixCVars
{
    public static readonly CVarDef<bool> RedirectCommonPrefix =
        CVarDef.Create("funkystation.redirect_common_prefix", true, CVar.SERVER | CVar.REPLICATED);
}
