using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Communications;

// TODO: you could probably turn this into something more generic
public sealed class PAKeyHolderSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PAKeyHolderComponent, EncryptionChannelsChangedEvent>(OnKeysChanged);
        SubscribeLocalEvent<PAKeyHolderComponent, ComponentStartup>(OnStartup);
    }

    private void OnKeysChanged(Entity<PAKeyHolderComponent> ent, ref EncryptionChannelsChangedEvent args)
    {
        UpdateChannels(ent, args.Component.Channels);
    }

    private void OnStartup(Entity<PAKeyHolderComponent> ent, ref ComponentStartup args)
    {
        if (TryComp<EncryptionKeyHolderComponent>(ent, out var keyHolder))
        {
            UpdateChannels(ent, keyHolder.Channels);
        }
    }

    private void UpdateChannels(Entity<PAKeyHolderComponent> ent, HashSet<ProtoId<RadioChannelPrototype>> channels)
    {
        if (TryComp<RadioSpeakerComponent>(ent, out var radioSpeaker))
        {
            radioSpeaker.Channels = channels;
            if (radioSpeaker.Enabled)
                EnsureComp<ActiveRadioComponent>(ent).Channels = channels;
        }
    }
}
