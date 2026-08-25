using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class PAAnnouncementCVars
{
    public static readonly CVarDef<bool> PAAnnouncements =
        CVarDef.Create("funkystation.chat.pa_announcements", true, CVar.SERVER | CVar.REPLICATED);

    public static readonly CVarDef<int> PAMaxAnnounceMessageLength =
        CVarDef.Create("funkystation.chat.pa_max_announce_message_length", 128, CVar.SERVER);
}
