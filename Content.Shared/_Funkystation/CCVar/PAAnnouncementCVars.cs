using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class PAAnnouncementCVars
{
    public static readonly CVarDef<bool> PAAnnouncements =
        CVarDef.Create("funkystation.chat.pa_announcements", true, CVar.SERVER | CVar.REPLICATED);
}
