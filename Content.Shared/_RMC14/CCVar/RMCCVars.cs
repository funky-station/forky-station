using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._RMC14.CCVar;

[CVarDefs]
public sealed class RMCCVars : CVars
{
    public static readonly CVarDef<float> RMCMentorHelpRateLimitPeriod =
        CVarDef.Create("rmc.mentor_help_rate_limit_period", 2f, CVar.SERVERONLY);

    public static readonly CVarDef<int> RMCMentorHelpRateLimitCount =
        CVarDef.Create("rmc.mentor_help_rate_limit_count", 10, CVar.SERVERONLY);

    public static readonly CVarDef<long> RMCDiscordMentorChatChannel =
        CVarDef.Create("rmc.discord_mentor_chat_channel", 0L, CVar.SERVER | CVar.SERVERONLY | CVar.CONFIDENTIAL);

    public static readonly CVarDef<string> RMCMentorHelpSound =
        CVarDef.Create("rmc.mentor_help_sound", "/Audio/_RMC14/Effects/Admin/mhelp.ogg", CVar.ARCHIVE | CVar.CLIENT | CVar.REPLICATED);

    public static readonly CVarDef<string> RMCMentorChatSound =
        CVarDef.Create("rmc.mentor_chat_sound", "/Audio/Items/pop.ogg", CVar.ARCHIVE | CVar.CLIENT | CVar.REPLICATED);

    public static readonly CVarDef<float> RMCMentorChatVolume =
        CVarDef.Create("rmc.mentor_help_volume", -5f, CVar.ARCHIVE | CVar.CLIENT | CVar.REPLICATED);
}
