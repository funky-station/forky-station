using Robust.Shared.Configuration;

namespace Content.Shared._Funkystation.CCVar;

[CVarDefs]
public sealed class PAAnnouncementCVars
{
    /// <summary>
    /// Enables PA speakers, allowing them to say announcements out loud.
    /// </summary>
    public static readonly CVarDef<bool> PAEnabled =
        CVarDef.Create("funkystation.pa.enabled", true, CVar.SERVER | CVar.REPLICATED,
            "Enable PA speakers, allowing them to say announcements out loud.");

    /// <summary>
    /// Makes announcements only audible from PA speakers when PAAnnouncementCVars.PAEnabled = true.
    /// </summary>
    public static readonly CVarDef<bool> PAExclusiveAnnouncements =
        CVarDef.Create("funkystation.pa.exclusive", true, CVar.SERVER | CVar.REPLICATED,
            "Make announcements only audible from PA speakers. Only has any effect when funkystation.pa.enabled = true.");

    public static readonly CVarDef<int> PAMaxAnnounceMessageLength =
        CVarDef.Create("funkystation.pa.max_announce_message_length", 128, CVar.SERVER);

    /// <summary>
    /// Helper method to check if PA speakers are enabled and announcements should be exclusively audible via them.
    /// </summary>
    public static bool IsPAEnabledAndExclusive(IConfigurationManager cfg)
    {
        return cfg.GetCVar(PAEnabled) && cfg.GetCVar(PAExclusiveAnnouncements);
    }
}
