using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Temperature.Components;
using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.SuitBreach;
using Content.Shared._Funkystation.SuitBreach.Components;
using Content.Shared._Funkystation.SuitBreach.EntitySystems;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Gravity;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Stacks;
using Content.Server.Stack;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.SuitBreach;

public sealed partial class SuitBreachSystem : SharedSuitBreachSystem
{
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private IRobustRandom _random = null!;
    [Dependency] private InventorySystem _inventory = null!;
    [Dependency] private GasTankSystem _gasTank = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private TagSystem _tag = null!;
    [Dependency] private SharedGravitySystem _gravity = null!;
    [Dependency] private SharedPhysicsSystem _physics = null!;
    [Dependency] private StackSystem _stack = null!;

    // qualifying damage types that can puncture a suit
    private static readonly ProtoId<DamageTypePrototype>[] PuncturingDamageTypes =
        ["Piercing", "Slash", "Heat"];

    private static readonly ProtoId<TagPrototype> SabotageTag = "Knife";

    private float _atmosAccumulator;
    private const float AtmosUpdateInterval = 1f;
    private bool _enabled;

    public override void Initialize()
    {
        base.Initialize();

        Subs.CVar(_cfg, SuitBreachCVars.Enabled, v => _enabled = v, true);

        SubscribeLocalEvent<DamageableComponent, DamageDealtEvent>(OnDamageDealt);
        SubscribeLocalEvent<InternalsComponent, InteractUsingEvent>(OnInteractUsingSuitedTarget);
        SubscribeLocalEvent<SuitBreachableComponent, InteractUsingEvent>(OnInteractUsingSuitItem);
        SubscribeLocalEvent<SuitBreachedComponent, SuitSealDoAfterEvent>(OnSealDoAfter);
        SubscribeLocalEvent<SuitBreachedComponent, ComponentShutdown>(OnBreachShutdown);
        SubscribeLocalEvent<SuitBreachedComponent, GetPressureProtectionValuesEvent>(OnGetPressureProtection);
        SubscribeLocalEvent<SuitBreachedComponent, GetTemperatureProtectionEvent>(OnGetTemperatureProtection);
        SubscribeLocalEvent<SuitBreachableComponent, SuitSabotageDoAfterEvent>(OnSabotageDoAfter);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _atmosAccumulator += frameTime;
        var doAtmos = false;
        var atmosDt = 0f;
        if (_atmosAccumulator >= AtmosUpdateInterval)
        {
            doAtmos = true;
            atmosDt = _atmosAccumulator;
            _atmosAccumulator -= AtmosUpdateInterval;
        }

        var query = EntityQueryEnumerator<SuitBreachedComponent>();
        while (query.MoveNext(out var suitUid, out var breach))
        {
            if (!_enabled)
            {
                StopHiss((suitUid, breach));
                if (breach.IsLeaking)
                {
                    breach.IsLeaking = false;
                    Dirty(suitUid, breach);
                }
                continue;
            }

            TickBreach((suitUid, breach), frameTime, doAtmos, atmosDt);
        }
    }

    // rolls a breach off a hit, escalates severity if it happens
    private void OnDamageDealt(EntityUid uid, DamageableComponent component, DamageDealtEvent args)
    {
        if (!_enabled)
            return;

        if (!_inventory.TryGetSlotEntity(uid, "outerClothing", out var suitUid))
            return;

        if (!TryComp<SuitBreachableComponent>(suitUid, out var breachable))
            return;

        var qualifyingDamage = 0f;
        foreach (var damageType in PuncturingDamageTypes)
        {
            if (args.Damage.DamageDict.TryGetValue(damageType, out var amount) && amount > 0)
                qualifyingDamage += (float)amount;
        }

        if (qualifyingDamage < breachable.MinDamageToRoll)
            return;

        if (!_random.Prob(breachable.BreachChance))
            return;

        var severity = SeverityFromDamage(qualifyingDamage);
        TryPuncture((suitUid.Value, null), severity, uid);
    }

    private void OnInteractUsingSuitedTarget(Entity<InternalsComponent> target, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!_inventory.TryGetSlotEntity(target.Owner, "outerClothing", out var suitUid))
            return;

        TryApplySealant(suitUid.Value, target.Owner, ref args);
    }

    private void OnInteractUsingSuitItem(Entity<SuitBreachableComponent> suit, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (_enabled && _tag.HasTag(args.Used, SabotageTag))
        {
            if (IsWorn(suit.Owner))
            {
                _popup.PopupEntity(Loc.GetString("suit-breach-sabotage-worn"), suit.Owner, args.User);
                args.Handled = true;
                return;
            }

            if (suit.Comp.SabotageAlwaysBreaches)
            {
                var doAfterArgs = new DoAfterArgs(EntityManager,
                    args.User,
                    suit.Comp.SabotageDelay,
                    new SuitSabotageDoAfterEvent(),
                    suit.Owner,
                    target: suit.Owner,
                    used: args.Used)
                {
                    BreakOnMove = true,
                    BreakOnDamage = true,
                    NeedHand = true,
                };

                args.Handled = _doAfter.TryStartDoAfter(doAfterArgs);
                return;
            }
        }

        TryApplySealant(suit.Owner, null, ref args);
    }

    private void OnSabotageDoAfter(Entity<SuitBreachableComponent> suit, ref SuitSabotageDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        args.Handled = true;

        if (IsWorn(suit.Owner))
        {
            _popup.PopupEntity(Loc.GetString("suit-breach-sabotage-worn"), suit.Owner, args.User);
            return;
        }

        var tempSuit = new Entity<SuitBreachedComponent?>(suit.Owner, null);
        if (TryEscalate(ref tempSuit, SuitBreachSeverity.Catastrophic) && tempSuit.Comp != null)
        {
            StopHiss((suit.Owner, tempSuit.Comp));
            StartHiss((suit.Owner, tempSuit.Comp));
            _popup.PopupEntity(Loc.GetString("suit-breach-sabotage-success"), suit.Owner, args.User);
        }
    }

    private void TryApplySealant(EntityUid suitUid, EntityUid? wearer, ref InteractUsingEvent args)
    {
        var isCanister = TryComp(args.Used, out SuitSealantCanisterComponent? canister);
        var isPatch = TryComp(args.Used, out SuitPatchComponent? patch);

        if (!isCanister && !isPatch)
            return;

        if (wearer == null && TryGetWearer(suitUid, out var actualWearer))
            wearer = actualWearer;

        if (!TryComp<SuitBreachedComponent>(suitUid, out _))
        {
            var message = wearer switch
            {
                null => Loc.GetString("suit-breach-seal-nothing-to-fix-item"),
                var w when w == args.User => Loc.GetString("suit-breach-seal-nothing-to-fix-self"),
                _ => Loc.GetString("suit-breach-seal-nothing-to-fix-other", ("target", Identity.Name(wearer.Value, EntityManager, args.User))),
            };
            _popup.PopupEntity(message, suitUid, args.User);
            args.Handled = true;
            return;
        }

        if (isCanister && canister is { Charges: <= 0 })
        {
            _popup.PopupEntity(Loc.GetString("suit-breach-seal-canister-empty"), suitUid, args.User);
            args.Handled = true;
            return;
        }

        var isSelf = wearer != null && wearer == args.User;
        TimeSpan delay;
        bool breakOnMove;

        if (isCanister && canister != null)
        {
            delay = isSelf ? canister.SelfApplyDelay : canister.OtherApplyDelay;
            breakOnMove = false;
        }
        else if (patch != null)
        {
            delay = isSelf ? patch.SelfApplyDelay : patch.OtherApplyDelay;
            breakOnMove = patch.BreakOnMove;
        }
        else
        {
            return;
        }

        var doAfterArgs = new DoAfterArgs(EntityManager,
            args.User,
            delay,
            new SuitSealDoAfterEvent(),
            suitUid,
            target: wearer ?? suitUid,
            used: args.Used)
        {
            BreakOnMove = breakOnMove,
            BreakOnDamage = true,
            NeedHand = true,
        };

        args.Handled = _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private bool TryGetWearer(EntityUid suitUid, out EntityUid wearer)
    {
        wearer = Transform(suitUid).ParentUid;
        return wearer != EntityUid.Invalid &&
               _inventory.TryGetSlotEntity(wearer, "outerClothing", out var equipped) &&
               equipped == suitUid;
    }

    private bool IsWorn(EntityUid suitUid)
    {
        return TryGetWearer(suitUid, out _);
    }

    private void OnSealDoAfter(Entity<SuitBreachedComponent> suit, ref SuitSealDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Used is not { } usedUid)
            return;

        var isCanister = TryComp(usedUid, out SuitSealantCanisterComponent? canister);
        var isPatch = HasComp<SuitPatchComponent>(usedUid);

        if (!isCanister && !isPatch)
            return;

        if (canister != null)
        {
            if (canister.Charges <= 0)
                return;

            canister.Charges--;
            Dirty(usedUid, canister);
            _audio.PlayPvs(canister.ApplySound, suit.Owner);
        }
        else if (TryComp(usedUid, out StackComponent? stackComp))
        {
            if (!_stack.TryUse((usedUid, stackComp), 1))
                return;
        }
        else
        {
            QueueDel(usedUid);
        }

        args.Handled = true;

        var newSeverity = Regress(suit);

        StopHiss(suit);
        if (newSeverity != SuitBreachSeverity.None)
            StartHiss(suit);

        var message = newSeverity == SuitBreachSeverity.None
            ? Loc.GetString("suit-breach-seal-fully-sealed")
            : Loc.GetString("suit-breach-seal-partially-sealed");
        _popup.PopupEntity(message, suit.Owner, args.User, PopupType.Medium);
    }

    private void TryPuncture(Entity<SuitBreachedComponent?> suit, SuitBreachSeverity severity, EntityUid? wearer)
    {
        if (!TryEscalate(ref suit, severity))
            return;

        if (suit.Comp != null)
        {
            StopHiss((suit.Owner, suit.Comp));
            StartHiss((suit.Owner, suit.Comp));
        }

        var target = wearer ?? Transform(suit.Owner).ParentUid;
        if (target != EntityUid.Invalid)
            _popup.PopupEntity(Loc.GetString("suit-breach-warning"), target, target, PopupType.LargeCaution);
    }

    private SuitBreachSeverity SeverityFromDamage(float damage)
    {
        if (damage >= _cfg.GetCVar(SuitBreachCVars.CatastrophicDamageThreshold))
            return SuitBreachSeverity.Catastrophic;

        if (damage >= _cfg.GetCVar(SuitBreachCVars.MajorDamageThreshold))
            return SuitBreachSeverity.Major;

        return SuitBreachSeverity.Minor;
    }

    // drains the connected tank at the current severity's rate
    private void TickBreach(Entity<SuitBreachedComponent> suit, float frameTime, bool doAtmos, float atmosDt)
    {
        var wearerUid = Transform(suit.Owner).ParentUid;
        if (!TryComp<InternalsComponent>(wearerUid, out var internals) ||
            !TryComp<GasTankComponent>(internals.GasTankEntity, out var tank))
        {
            StopHiss(suit);
            if (!suit.Comp.IsLeaking)
                return;
            suit.Comp.IsLeaking = false;
            Dirty(suit, suit.Comp);
            return;
        }

        if (doAtmos)
        {
            var rate = suit.Comp.DrainRatesPerSecond.GetValueOrDefault(suit.Comp.Severity, 0f);
            if (rate > 0 && tank.Air.TotalMoles > 0)
            {
                _gasTank.RemoveAir((internals.GasTankEntity.Value, tank), rate * atmosDt);
            }
        }

        var hasGasLeft = tank.Air.TotalMoles > 0;
        if (hasGasLeft)
        {
            StartHiss(suit);
            if (!suit.Comp.IsLeaking)
            {
                suit.Comp.IsLeaking = true;
                Dirty(suit, suit.Comp);
            }

            // weightless wearer gets pushed by the venting gas
            if (!_gravity.IsWeightless(wearerUid) || !TryComp<PhysicsComponent>(wearerUid, out var physics))
                return;

            var impulse = suit.Comp.LeakImpulsePerSecond.GetValueOrDefault(suit.Comp.Severity, 0f);

            if (!(impulse > 0f))
                return;

            _physics.WakeBody(wearerUid, body: physics);
            var direction = suit.Comp.LeakAngle.ToVec();
            _physics.ApplyLinearImpulse(wearerUid, direction * impulse * frameTime, body: physics);
        }
        else
        {
            StopHiss(suit);
            if (!suit.Comp.IsLeaking)
                return;

            suit.Comp.IsLeaking = false;
            Dirty(suit, suit.Comp);
        }
    }

    private void StartHiss(Entity<SuitBreachedComponent> suit)
    {
        if (suit.Comp.HissStream != null)
            return;

        var volume = suit.Comp.HissVolumePerSeverity.GetValueOrDefault(suit.Comp.Severity, 0f);
        var audioParams = AudioParams.Default.WithLoop(true).WithVolume(volume);

        if (_audio.PlayPvs(suit.Comp.HissSound, suit.Owner, audioParams) is { } hissPlayed)
            suit.Comp.HissStream = hissPlayed.Entity;
    }

    private void StopHiss(Entity<SuitBreachedComponent> suit)
    {
        if (suit.Comp.HissStream == null)
            return;

        _audio.Stop(suit.Comp.HissStream);
        suit.Comp.HissStream = null;
    }

    private void OnBreachShutdown(Entity<SuitBreachedComponent> ent, ref ComponentShutdown args)
    {
        StopHiss(ent);
    }

    private void OnGetPressureProtection(Entity<SuitBreachedComponent> ent, ref GetPressureProtectionValuesEvent args)
    {
        if (ent.Comp.Severity != SuitBreachSeverity.Catastrophic)
            return;

        args.HighPressureMultiplier = 1f;
        args.HighPressureModifier = 0f;
        args.LowPressureMultiplier = 1f;
        args.LowPressureModifier = 0f;
    }

    private void OnGetTemperatureProtection(Entity<SuitBreachedComponent> ent, ref GetTemperatureProtectionEvent args)
    {
        if (ent.Comp.Severity != SuitBreachSeverity.Catastrophic)
            return;

        args.Coefficient = 1f;
    }
}
