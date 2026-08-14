using Content.Shared.Radio;
using Content.Shared.Radio.Components;

namespace Content.Shared._Funkystation.Communications;

public sealed class PAKeyHolderSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PAKeyHolderComponent, EncryptionChannelsChangedEvent>(OnKeysChanged);
    }

    private void OnKeysChanged(Entity<PAKeyHolderComponent> ent, ref EncryptionChannelsChangedEvent args)
    {
        if (TryComp<RadioSpeakerComponent>(ent, out var radioSpeaker))
        {
            radioSpeaker.Channels = args.Component.Channels;
            if (radioSpeaker.Enabled)
                EnsureComp<ActiveRadioComponent>(ent).Channels = args.Component.Channels;
        }
    }
}
