using Content.Server.Chat;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Chat;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Server.Chat.Systems;

public sealed partial class AnnounceOnSpawnSystem : EntitySystem
{
    [Dependency] private ChatSystem _chat = default!;
    [Dependency] private AudioSystem _audio = null!; // funky

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnnounceOnSpawnComponent, MapInitEvent>(OnInit);
    }

    private void OnInit(EntityUid uid, AnnounceOnSpawnComponent comp, MapInitEvent args)
    {
        var message = Loc.GetString(comp.Message);
        var sender = comp.Sender != null ? Loc.GetString(comp.Sender) : Loc.GetString("chat-manager-sender-announcement");

        var announcement = new ChatAnnouncement(
            Message: message,
            SenderName: sender,
            ShouldPlaySound: !comp.GlobalSound, // avoid playing the spooky nar'sie noise through PA speakers, it should be heard globally instead
            AltAnnouncementSound: comp.Sound,
            ColorOverride: comp.Color);

        _chat.DispatchGlobalAnnouncement(announcement);

        if (comp.GlobalSound)
            _audio.PlayGlobal(comp.Sound, Filter.Broadcast(), true, AudioParams.Default.WithVolume(-2f));
    }
}
