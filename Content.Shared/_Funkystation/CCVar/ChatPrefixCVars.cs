using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class ChatPrefixCVars
{
    /// <summary>
    /// Whether typing ";" before a message should try
    /// to send a message to the common channel or the
    /// speaker's default channel.
    /// </summary>
    public static readonly CVarDef<bool> RedirectCommonPrefix =
        CVarDef.Create("funkystation.chat.redirect_common_prefix", true, CVar.SERVER | CVar.REPLICATED);
}
