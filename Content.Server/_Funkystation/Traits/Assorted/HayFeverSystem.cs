using System.ComponentModel;
using Content.Server.Chat.Systems;
using Content.Shared.Bed.Sleep;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Speech;
using Content.Shared.Speech.Muting;
using Content.Shared._Funkystation.Traits.Assorted;
using Content.Shared.StatusEffectNew;
using NetCord;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.Traits.Assorted;

public sealed partial class HayFeverSystem : EntitySystem
{

    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private ChatSystem _chatSystem = default!;
    [Dependency] private MobStateSystem _mobstateSystem = default!;
    [Dependency] private StatusEffectsSystem _statusEffects = default!;
    [Dependency] private IRobustRandom _random = default!;
    private const float UpdateInterval = 1f;
    private float _updateTimer;
    public override void Initialize()
    {
        SubscribeLocalEvent<HayFeverComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, HayFeverComponent allergy, ComponentStartup args)
    {
        allergy.TimeSinceReaction = 0f;
        allergy.NextReactionTime = allergy.ReactionInterval;
        allergy.NextSneezeTime = 0f;
        allergy.SneezeDelay = 1f;
        allergy.SneezesQueued = 0;
    }

    /// <summary>
    ///  Updates the HayFeverSystem
    /// </summary>
    /// <param name="frameTime">Time in seconds covered by the current game tick.</param>
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        _updateTimer += frameTime;

        while (_updateTimer >= UpdateInterval)
        {
            _updateTimer -= UpdateInterval;

            var hayfeverQuery = EntityQueryEnumerator<HayFeverComponent>();
            while (hayfeverQuery.MoveNext(out var uid, out var allergy))
            {
                if (_mobstateSystem.IsIncapacitated(uid) || TryComp<SleepingComponent>(uid, out _))
                    continue;
                allergy.TimeSinceReaction += UpdateInterval;
                AllergicReaction(uid, allergy);
                Sneeze(uid, allergy);
            }
        }

    }
    /// <summary>
    /// If enough time has passed since the last allergic reaction, attempts to roll an allergic reaction.
    /// </summary>
    /// <param name="uid">the unique ID of the player character.</param>
    /// <param name="allergy">The instanced HayFeverComponent of the player character.</param>
    private void AllergicReaction(EntityUid uid, HayFeverComponent allergy)
    {

        if (allergy.TimeSinceReaction <= allergy.NextReactionTime)
            return;

        allergy.ReactionType = _random.Next(0, 25);
        switch (allergy.ReactionType)
        {
            // If 0-10 is rolled, nothing happens, this is intended behavior to make allergic reaction less predictable.

            case >= 11 and <= 12: // Itchy nose
                _popup.PopupEntity(Loc.GetString("trait-hayfever-popup1"), uid, uid, PopupType.Medium);
                break;
            case >= 13 and <= 14: // Itchy eyes
                _popup.PopupEntity(Loc.GetString("trait-hayfever-popup2"), uid, uid,  PopupType.Medium);
                break;
            case >= 15 and <= 22: // One sneeze
                _popup.PopupEntity(Loc.GetString("trait-hayfever-popup3"), uid, uid, PopupType.MediumCaution);
                allergy.SneezesQueued = 1;
                break;
            case >= 23: // Sneeze attack consisting of 2-4 sneezes
                _popup.PopupEntity(Loc.GetString("trait-hayfever-popup4"), uid, uid, PopupType.LargeCaution);
                allergy.SneezesQueued = _random.Next(2,5);
                break;

        }

        allergy.TimeSinceReaction = 0f;
        allergy.NextReactionTime = allergy.ReactionInterval;
        allergy.NextSneezeTime = allergy.TimeSinceReaction;
    }
    /// <summary>
    /// If enough time has passed since a sneeze was queued, and at least one sneeze is queued, sneezes once and subtracts 1 from the number of sneezes queued.
    /// </summary>
    /// <param name="uid">The unique ID of the player character.</param>
    /// <param name="allergy">The instanced HayFeverComponent of the player character.</param>
    private void Sneeze(EntityUid uid, HayFeverComponent allergy)
    {
        if (allergy.TimeSinceReaction <= allergy.NextSneezeTime || allergy.SneezesQueued < 1)
            return;
        if (TryComp<SpeechComponent>(uid, out _) && !_statusEffects.HasEffectComp<MutedStatusEffectComponent>(uid))
            _chatSystem.TryEmoteWithChat(uid, "Sneeze");
        allergy.SneezesQueued -= 1;
    }
}
