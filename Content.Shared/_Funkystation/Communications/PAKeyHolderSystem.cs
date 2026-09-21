using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._Funkystation.Communications;

// TODO: you could probably turn this into something more generic
public sealed partial class PAKeyHolderSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = null!;

    [SubscribeLocalEvent]
    private void OnKeysChanged(Entity<PAKeyHolderComponent> ent, ref EncryptionChannelsChangedEvent args)
    {
        UpdateChannels(ent, args.Component.Channels);
    }

    private void UpdateChannels(Entity<PAKeyHolderComponent> ent, HashSet<ProtoId<RadioChannelPrototype>> channels)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        if (TryComp<RadioSpeakerComponent>(ent, out var radioSpeaker))
        {
            if (!radioSpeaker.Enabled || MetaData(ent).EntityLifeStage >= EntityLifeStage.Terminating)
                return;

            if (channels.Count == 0)
                RemComp<ActiveRadioComponent>(ent);
            else
                EnsureComp<ActiveRadioComponent>(ent).Channels = new(channels);
        }
    }
}
