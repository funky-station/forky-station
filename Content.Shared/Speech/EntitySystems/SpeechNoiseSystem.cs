using Content.Shared.Chat;
using Content.Shared.Random.Helpers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Speech.EntitySystems;

public sealed partial class SpeechSoundSystem : EntitySystem
{
    [Dependency] private IGameTiming _gameTiming = default!;
    [Dependency] private INetManager _net = default!;
    [Dependency] private SharedAudioSystem _audio = default!;

    [SubscribeLocalEvent]
    private void OnEntitySpoke(Entity<SpeechComponent> ent, ref EntitySpokeEvent args)
    {
        if (ent.Comp.SpeechSounds == null)
            return;

        var currentTime = _gameTiming.CurTime;
        var cooldown = TimeSpan.FromSeconds(ent.Comp.SoundCooldownTime);

        // Ensure more than the cooldown time has passed since last speaking
        if (currentTime - ent.Comp.LastTimeSoundPlayed < cooldown)
            return;

        var sound = GetSpeechSound(ent, args.Message);
        ent.Comp.LastTimeSoundPlayed = currentTime;
        if (_net.IsServer) // TODO: replace this call with PlayPredicted when chat is predicted.
            _audio.PlayPvs(sound, ent);
    }

    /// <summary>
    /// Gets the speech sound for a message.
    /// </summary>
    public SoundSpecifier? GetSpeechSound(Entity<SpeechComponent> ent, string message)
    {
        // MACRO Start: SpeechSounds
        //if (ent.Comp.SpeechSounds == null)
        //    return null;
        var protoId = ent.Comp.SpeechSounds;

        // raise event for voice-changing equipment
        var voiceEv = new TransformSpeakerVoiceEvent(ent);
        RaiseLocalEvent(ent, voiceEv);
        protoId = voiceEv.SpeechSounds ?? protoId;

        if (protoId == null)
            return null;
        // MACRO End: SpeechSounds

        // Play speech sound
        var prototype = ProtoMan.Index<SpeechSoundsPrototype>(protoId); // MACRO: SpeechSounds, change to protoId

        // Different sounds for ask/exclaim based on last character
        var contextSound = message[^1] switch
        {
            '?' => prototype.AskSound,
            '!' => prototype.ExclaimSound,
            _ => prototype.SaySound
        };

        // Use exclaim sound if most characters are uppercase.
        var uppercaseCount = 0;
        foreach (var t in message)
        {
            if (char.IsUpper(t))
                uppercaseCount++;
        }

        if (uppercaseCount > message.Length / 2)
        {
            contextSound = prototype.ExclaimSound;
        }

        var random = SharedRandomExtensions.PredictedRandom(_gameTiming, GetNetEntity(ent));
        var scale = (float)random.NextGaussian(1, prototype.Variation);
        contextSound.Params = ent.Comp.AudioParams.WithPitchScale(scale);
        return contextSound;
    }
}
