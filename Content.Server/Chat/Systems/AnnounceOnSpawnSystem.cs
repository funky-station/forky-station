using Content.Server.Chat;
using Content.Shared._Funkystation.CCVar;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Server.Chat.Systems;

public sealed partial class AnnounceOnSpawnSystem : EntitySystem
{
    [Dependency] private ChatSystem _chat = default!;
    [Dependency] private IConfigurationManager _cfg = null!; // funky - pa announcement cvar
    [Dependency] private AudioSystem _audio = null!; // funky

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnnounceOnSpawnComponent, MapInitEvent>(OnInit);
    }

    private void OnInit(EntityUid uid, AnnounceOnSpawnComponent comp, MapInitEvent args)
    {
        var paExclusive = PAAnnouncementCVars.IsPAEnabledAndExclusive(_cfg); // funky
        var message = Loc.GetString(comp.Message);
        var sender = comp.Sender != null ? Loc.GetString(comp.Sender) : Loc.GetString("chat-manager-sender-announcement");
        // funky - because i see this component being used for nar'sie and rat'var, let's preserve the default global sound behaviour
        _chat.DispatchGlobalAnnouncement(message, sender, playSound: true,
            paExclusive ? null : comp.Sound, // funky
            comp.Color);
        // funky
        if (paExclusive)
            _audio.PlayGlobal(comp.Sound, Filter.Broadcast(), true, AudioParams.Default.WithVolume(-2f));
    }
}
