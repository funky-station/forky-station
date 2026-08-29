using Content.Shared._Funkystation.CCVar;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Traits.Assorted;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.Cpr;

/// <summary>
/// used for handling CPR on crit or dead mobs
/// </summary>
public abstract partial class SharedCprSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _config = null!;
    [Dependency] private DamageableSystem _damage = null!;
    [Dependency] private SharedInteractionSystem _interactionSystem = null!;
    [Dependency] private InventorySystem _inventory = null!;
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] protected EntityManager Ent = null!;
    [Dependency] protected IGameTiming Timing = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private IRobustRandom _random = null!;
    [Dependency] private MobThresholdSystem _mobThreshold = null!;
    [Dependency] private MobStateSystem _mobState = null!;
    [Dependency] private SharedRottingSystem _rotting = null!;
    [Dependency] private INetManager _net = null!;

    private const float CprInteractionRangeMultiplier = 0.25f;
    protected const float CprAnimationLength = 0.2f;
    protected const float CprAnimationEndTime = 1f;
    private const float CprManualEffectDuration = 5f;
    private const float CprManualThreshold = 1.5f;

    private const string AirlossDamageType = "Asphyxiation";
    private const string RibCrackDamageType = "Blunt";

    private bool _cprRepeat;
    private float _cprReviveChance;
    private float _cprAirlossHealAmount;
    private float _cprDoAfterDelay;
    private int _cprRibCrackPump;
    private float _cprRibCrackDamage;

    public override void Initialize()
    {
        base.Initialize();

        _config.OnValueChanged(CprCVars.Repeat, value => _cprRepeat = value, true);
        _config.OnValueChanged(CprCVars.ReviveChance, value => _cprReviveChance = value, true);
        _config.OnValueChanged(CprCVars.AirlossHealAmount, value => _cprAirlossHealAmount = value, true);
        _config.OnValueChanged(CprCVars.DoAfterDelay, value => _cprDoAfterDelay = value, true);
        _config.OnValueChanged(CprCVars.RibCrackPump, value => _cprRibCrackPump = value, true);
        _config.OnValueChanged(CprCVars.RibCrackDamage, value => _cprRibCrackDamage = value, true);

        SubscribeLocalEvent<CprComponent, GetVerbsEvent<AlternativeVerb>>(OnGetAlternativeVerbs);
        SubscribeLocalEvent<CprComponent, CprDoAfterEvent>(OnCprDoAfter);

        SubscribeLocalEvent<CprComponent, GetInteractingEntitiesEvent>(OnGetInteractingEntities);

        SubscribeLocalEvent<CprComponent, MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnMobStateChanged(Entity<CprComponent> ent, ref MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Alive)
            return;

        ent.Comp.HasCrackedRibsThisCrit = false;
        ent.Comp.PumpsThisCrit = 0;
    }

    private void OnGetInteractingEntities(Entity<CprComponent> ent, ref GetInteractingEntitiesEvent args)
    {
        if (ent.Comp.LastCaretaker is { } user && !CprCaretakerOutdated(ent.Comp))
        {
            args.InteractingEntities.Add(user);
        }
    }

    private bool CprCaretakerOutdated(CprComponent cpr)
    {
        return (Timing.CurTime - cpr.LastTimeGivenCare).TotalSeconds > _cprDoAfterDelay;
    }

    private bool CanDoCpr(EntityUid recipient, EntityUid giver)
    {
        if (!HasComp<CprComponent>(recipient))
            return false;

        if (!_mobState.IsIncapacitated(recipient))
            return false;

        return !_mobState.IsIncapacitated(giver);
    }

    private bool InRangeForCpr(EntityUid recipient, EntityUid giver)
    {
        return _interactionSystem.InRangeUnobstructed(giver, recipient, SharedInteractionSystem.InteractionRange * CprInteractionRangeMultiplier);
    }

    private void OnCprDoAfter(Entity<CprComponent> ent, ref CprDoAfterEvent args)
    {
        if (args.Handled)
            return;

        if (args.Cancelled)
        {
            ent.Comp.LastCaretaker = null;
            ent.Comp.LastTimeGivenCare = TimeSpan.Zero;
            Dirty(ent, ent.Comp);
            return;
        }

        if (!CanDoCpr(ent, args.User))
            return;

        if (_inventory.TryGetSlotEntity(ent, "outerClothing", out _))
            return;

        if (!TryComp<DamageableComponent>(ent, out var damage) ||
            !TryComp<CprComponent>(ent, out var cpr) ||
            !TryComp<MobStateComponent>(ent, out var mobState))
            return;

        DoLunge(args.User);

        _audio.PlayPredicted(cpr.Sound, ent.Owner, args.User);

        // heal asphyxiation per pump
        if (_cprAirlossHealAmount > 0)
        {
            var airlossHeal = new DamageSpecifier();
            airlossHeal.DamageDict.Add(AirlossDamageType, -_cprAirlossHealAmount);
            _damage.TryChangeDamage((ent.Owner, damage), airlossHeal, ignoreResistances: true, interruptsDoAfters: false);
        }

        // if the patient is dead, roll for a revive chance
        if (_mobState.IsDead(ent.Owner, mobState))
        {
            var totalDamage = _damage.GetTotalDamage((ent.Owner, damage)); // I'll worry about it when a new med system comes out :godo:

            var hasDeadThreshold = _mobThreshold.TryGetThresholdForState(ent.Owner, MobState.Dead, out var deadThreshold);
            var isHealedEnough = !hasDeadThreshold || totalDamage < deadThreshold;

            // server only roll to prevent client mispredicting
            if (_net.IsServer &&
                !HasComp<UnrevivableComponent>(ent) &&
                !_rotting.IsRotten(ent) &&
                isHealedEnough &&
                _random.Prob(_cprReviveChance))
            {
                var targetState = MobState.Alive;

                if (_mobThreshold.TryGetThresholdForState(ent.Owner, MobState.Critical, out var critThreshold) &&
                    totalDamage > critThreshold)
                {
                    targetState = MobState.Critical;
                }

                _mobState.ChangeMobState(ent.Owner, targetState, mobState);

                if (_mobState.IsCritical(ent.Owner, mobState) || _mobState.IsAlive(ent.Owner, mobState))
                {
                    _popup.PopupPredicted(Loc.GetString("cpr-revive-success", ("target", ent.Owner)), args.User, args.User);
                }
            }
        }

        // ribs crack after reaching pump threshold
        cpr.PumpsThisCrit++;
        if (!cpr.HasCrackedRibsThisCrit && cpr.PumpsThisCrit >= _cprRibCrackPump)
        {
            cpr.HasCrackedRibsThisCrit = true;

            if (_cprRibCrackDamage > 0)
            {
                var crackDamage = new DamageSpecifier();
                crackDamage.DamageDict.Add(RibCrackDamageType, _cprRibCrackDamage);
                _damage.TryChangeDamage((ent.Owner, damage), crackDamage, ignoreResistances: true, interruptsDoAfters: false);
            }

            _popup.PopupPredicted(
                Loc.GetString("cpr-rib-crack-you", ("target", ent.Owner)),
                Loc.GetString("cpr-rib-crack-others", ("person", args.User), ("target", ent.Owner)),
                ent.Owner,
                args.User,
                PopupType.MediumCaution);

            _audio.PlayPredicted(CprRibCrackSound, ent.Owner, args.User);
        }

        var assist = EnsureComp<AssistedRespirationComponent>(ent);

        var newUntil = _cprRepeat
            ? Timing.CurTime + TimeSpan.FromSeconds(_cprDoAfterDelay + 0.25f)
            : Timing.CurTime + TimeSpan.FromSeconds(CprManualEffectDuration);

        if (newUntil > assist.AssistedUntil)
            assist.AssistedUntil = newUntil;

        // if they are NOT incapacitated apply bonus healing
        if (!_mobState.IsIncapacitated(ent.Owner, mobState))
        {
            var healing = new DamageSpecifier(cpr.BonusHeal);

            _damage.TryChangeDamage((ent.Owner, damage), healing, ignoreResistances: true, interruptsDoAfters: false);
        }

        cpr.LastCaretaker = args.User;
        cpr.LastTimeGivenCare = Timing.CurTime;
        Dirty(ent, cpr);

        args.Repeat = _mobState.IsIncapacitated(ent.Owner, mobState) && _cprRepeat;
        args.Handled = true;
    }

    public abstract void DoLunge(EntityUid user);

    private void TryStartCpr(EntityUid recipient, EntityUid giver)
    {
        if (!TryComp<CprComponent>(recipient, out var cpr))
            return;

        if (!CanDoCpr(recipient, giver) || !InRangeForCpr(recipient, giver))
            return;

        if (_inventory.TryGetSlotEntity(recipient, "outerClothing", out _))
        {
            _popup.PopupClient(Loc.GetString("cpr-clothing-blocking"), recipient, giver);
            return;
        }

        var interactingEntities = new HashSet<EntityUid>();
        _interactionSystem.GetEntitiesInteractingWithTarget(recipient, interactingEntities);

        interactingEntities.Remove(giver);

        if (interactingEntities.Count > 0)
        {
            _popup.PopupClient(Loc.GetString("cpr-already-in-progress"), recipient, giver);
            return;
        }

        cpr.LastCaretaker = giver;
        cpr.LastTimeGivenCare = Timing.CurTime;
        Dirty(recipient, cpr);

        var doAfterEventArgs = new DoAfterArgs(
            EntityManager,
            giver,
            TimeSpan.FromSeconds(_cprDoAfterDelay),
            new CprDoAfterEvent(),
            recipient,
            recipient
            )
        {
            BreakOnMove = true,
            DistanceThreshold = SharedInteractionSystem.InteractionRange * CprInteractionRangeMultiplier,
            BlockDuplicate = true,
            DuplicateCondition = DuplicateConditions.SameTarget,
            RequireCanInteract = true,
            NeedHand = true
        };

        if (!_doAfter.TryStartDoAfter(doAfterEventArgs))
        {
            cpr.LastCaretaker = null;
            cpr.LastTimeGivenCare = TimeSpan.Zero;
            Dirty(recipient, cpr);
            return;
        }

        var timeLeft = TimeSpan.Zero;
        if (TryComp<AssistedRespirationComponent>(recipient, out var comp))
            timeLeft = comp.AssistedUntil - Timing.CurTime;

        var recommendedRate = Math.Round(CprManualEffectDuration - CprManualThreshold);
        if (comp is null)
        {
            var localString = Loc.GetString("cpr-start-you", ("target", Identity.Entity(recipient, EntityManager)));
            var othersString = Loc.GetString("cpr-start", ("person", Identity.Entity(giver, EntityManager)), ("target", Identity.Entity(recipient, EntityManager)));
            _popup.PopupPredicted(localString, othersString, giver, giver, PopupType.Medium);
        }
        else if (!_cprRepeat && timeLeft <= TimeSpan.Zero)
        {
            _popup.PopupCursor(Loc.GetString("cpr-too-slow", ("seconds", recommendedRate)), giver, PopupType.Large);
        }
        else if (timeLeft > TimeSpan.FromSeconds(CprManualEffectDuration - CprManualThreshold))
        {
            _popup.PopupCursor(Loc.GetString("cpr-too-fast", ("seconds", recommendedRate)), giver, PopupType.Large);
        }
    }

    private void OnGetAlternativeVerbs(EntityUid uid, CprComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!CanDoCpr(uid, args.User))
            return;

        var inRange = InRangeForCpr(uid, args.User);

        var verb = new AlternativeVerb()
        {
            Act = () =>
            {
                TryStartCpr(uid, args.User);
            },
            Text = Loc.GetString("cpr-verb-text"),
            Priority = 5,
            Disabled = !inRange,
            Message = inRange ? null : Loc.GetString("cpr-verb-text-disabled"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/_Funkystation/Interface/VerbIcons/cpr.svg.192dpi.png"))
        };

        args.Verbs.Add(verb);
    }

    private static readonly SoundSpecifier CprRibCrackSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/ribcrack.ogg");
}

[Serializable, NetSerializable]
public sealed partial class CprDoAfterEvent : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}
