using Content.Shared.Atmos.Rotting;
using Content.Shared.Chat;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Electrocution;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.PowerCell;
using Content.Shared.Timing;
using Content.Shared.Traits.Assorted;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;
// funky start
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Linq;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Content.Shared.Inventory;
using Content.Shared.FixedPoint;
using Content.Shared.EntityEffects.Effects.StatusEffects;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Body.Components;
using Content.Shared._Funkystation.CCVar;
using Content.Shared.Damage;
// funky end

namespace Content.Shared.Medical;

/// <summary>
/// This handles interactions and logic relating to <see cref="DefibrillatorComponent"/>
/// </summary>
public abstract partial class SharedDefibrillatorSystem : EntitySystem
{
    [Dependency] private SharedChatSystem _chat = default!;
    [Dependency] private DamageableSystem _damageable = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private SharedElectrocutionSystem _electrocution = default!;
    [Dependency] private ISharedPlayerManager _player = default!;
    [Dependency] private ItemToggleSystem _toggle = default!;
    [Dependency] private MobStateSystem _mobState = default!;
    [Dependency] private MobThresholdSystem _mobThreshold = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private PowerCellSystem _powerCell = default!;
    [Dependency] private SharedRottingSystem _rotting = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedMindSystem _mind = default!;
    [Dependency] private UseDelaySystem _useDelay = default!;
    [Dependency] private SharedInteractionSystem _interaction = default!;
    [Dependency] private InventorySystem _inventory = default!; // funky
    [Dependency] private SharedSolutionContainerSystem _solutionContainer = default!; // funky
    [Dependency] private IRobustRandom _random = default!; // funky
    [Dependency] private IPrototypeManager _prototypeManager = default!; // funky
    [Dependency] private IConfigurationManager _config = default!; // funky
    [Dependency] private INetManager _net = default!; // funky

    private readonly HashSet<EntityUid> _interactors = new();
    private float _reviveChance; // funky
    private float _adrenalineCostPerShock; // funky

    public override void Initialize()
    {
        base.Initialize(); // funky
        _config.OnValueChanged(DefibrillatorCVars.ReviveChance, value => _reviveChance = value, true); // funky
        _config.OnValueChanged(DefibrillatorCVars.AdrenalineCost, value => _adrenalineCostPerShock = value, true); // funky

        SubscribeLocalEvent<DefibrillatorComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(Entity<DefibrillatorComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Handled || args.Target is not { } target)
            return;

        args.Handled = TryStartZap(ent.AsNullable(), target, args.User);
    }

    [SubscribeLocalEvent]
    private void OnDoAfter(Entity<DefibrillatorComponent> ent, ref DefibrillatorZapDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        if (args.Target is not { } target)
            return;

        if (!CanZap(ent.AsNullable(), target, args.User))
            return;

        args.Handled = true;
        Zap(ent.AsNullable(), target, args.User);
    }

    /// <summary>
    /// Checks if you can actually defib a target.
    /// </summary>
    /// <param name="ent">The defbrillator being used.</param>
    /// <param name="target">Uid of the target getting defibbed.</param>
    /// <param name="user">Uid of the entity using the defibrillator.</param>
    /// <returns>
    /// Returns true if the target is valid to be defibed, false otherwise.
    /// </returns>
    public bool CanZap(Entity<DefibrillatorComponent?> ent, EntityUid target, EntityUid? user = null)
    {
        if (!Resolve(ent, ref ent.Comp))
            return false;

        if (!_toggle.IsActivated(ent.Owner))
        {
            _popup.PopupEntity(Loc.GetString("defibrillator-not-on"), ent.Owner, user);
            return false;
        }

        if (!TryComp<UseDelayComponent>(ent, out var useDelay) || _useDelay.IsDelayed((ent.Owner, useDelay), ent.Comp.DelayId))
            return false;

        if (!_powerCell.HasActivatableCharge(ent.Owner, user: user, predicted: true))
            return false;

        // funky, gotta take off their hardsuit or coat
        if (!_inventory.TryGetSlotEntity(target, "outerClothing", out _))
            return true;

        _popup.PopupClient(Loc.GetString("defibrillator-clothing-blocking"), user);
        return false;

    }

    /// <summary>
    /// Tries to start defibrillating the target. If the target is valid, will start the defib do-after.
    /// </summary>
    /// <param name="ent">The defbrillator being used.</param>
    /// <param name="target">Uid of the target getting defibbed.</param>
    /// <param name="user">Uid of the entity using the defibrillator.</param>
    /// <returns>
    /// Returns true if the defibrillation do-after started, otherwise false.
    /// </returns>
    public bool TryStartZap(Entity<DefibrillatorComponent?> ent, EntityUid target, EntityUid user)
    {
        if (!Resolve(ent, ref ent.Comp))
            return false;

        if (!CanZap(ent, target, user))
            return false;

        _audio.PlayPredicted(ent.Comp.ChargeSound, ent.Owner, user);
        _popup.PopupEntity(Loc.GetString("defibrillator-begin", ("name", Identity.Entity(user, EntityManager)), ("target", Identity.Entity(target, EntityManager))), target, PopupType.SmallCaution);

        return _doAfter.TryStartDoAfter(
            new DoAfterArgs(EntityManager, user, ent.Comp.DoAfterDuration, new DefibrillatorZapDoAfterEvent(),
            ent.Owner, target, ent.Owner)
            {
                NeedHand = true,
                BreakOnMove = !ent.Comp.AllowDoAfterMovement
            });
    }

    /// <summary>
    /// Tries to defibrillate the target with the given defibrillator.
    /// </summary>
    /// <param name="ent">The defbrillator being used.</param>
    /// <param name="target">Uid of the target getting defibbed.</param>
    /// <param name="user">Uid of the entity using the defibrillator.</param>
    public void Zap(Entity<DefibrillatorComponent?> ent, EntityUid target, EntityUid user)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        if (!_powerCell.TryUseActivatableCharge(ent.Owner, user: user))
            return;

        var selfEvent = new SelfBeforeDefibrillatorZapsEvent(user, ent.Owner, target);
        RaiseLocalEvent(user, selfEvent);

        target = selfEvent.DefibTarget;

        // Ensure thet new target is still valid.
        if (selfEvent.Cancelled || !CanZap(ent, target, user))
            return;

        var targetEvent = new TargetBeforeDefibrillatorZapsEvent(user, ent.Owner, target);
        RaiseLocalEvent(target, targetEvent);

        target = targetEvent.DefibTarget;

        if (targetEvent.Cancelled || !CanZap(ent, target, user))
            return;

        if (TryComp<UseDelayComponent>(ent, out var useDelay))
        {
            _useDelay.SetLength((ent.Owner, useDelay), ent.Comp.ZapDelay, id: ent.Comp.DelayId);
            _useDelay.TryResetDelay((ent.Owner, useDelay), id: ent.Comp.DelayId);
        }

        _audio.PlayPredicted(ent.Comp.ZapSound, ent, user);
        Entity<DefibrillatorComponent> defibEnt = (ent, ent.Comp);
        var failedRevive = TryRevive(defibEnt, user, target, true);

        _interaction.GetEntitiesInteractingWithTarget(target, _interactors);
        foreach (var interactor in _interactors)
        {
            TryRevive(defibEnt, user, interactor, false);
        }

        var sound = failedRevive
            ? ent.Comp.FailureSound
            : ent.Comp.SuccessSound;
        _audio.PlayPredicted(sound, ent.Owner, user);

        var ev = new TargetDefibrillatedEvent(user, target, (ent.Owner, ent.Comp), _interactors);
        RaiseLocalEvent(target, ref ev);

        // if we don't have enough power left for another shot, turn it off
        if (!_powerCell.HasActivatableCharge(ent.Owner))
            _toggle.TryDeactivate(ent.Owner);
    }

    private bool TryRevive(Entity<DefibrillatorComponent> ent, EntityUid user, EntityUid target, bool isOriginal)
    {
        bool failedRevive = true;
        if (_rotting.IsRotten(target))
        {
            _chat.TrySendInGameICMessage(ent.Owner, Loc.GetString("defibrillator-rotten"), InGameICChatType.Speak, true);
        }
        else if (TryComp<UnrevivableComponent>(target, out var unrevivable))
        {
            _chat.TrySendInGameICMessage(ent.Owner, Loc.GetString(unrevivable.ReasonMessage), InGameICChatType.Speak, true);
        }
        else
        {
            TryComp<MobStateComponent>(target, out var targetMobState);

            _damageable.TryChangeDamage(target, ent.Comp.ZapHeal, true, origin: user);

            // funky start, need an adrenaline reagent in their system to kick the heart back on
            var hasAdrenaline = false;
            if (TryComp<BloodstreamComponent>(target, out var bloodstream))
            {
                var bloodSolution = bloodstream.BloodSolution;

                if (_solutionContainer.ResolveSolution(target, bloodstream.BloodSolutionName, ref bloodSolution))
                {
                    var contents = bloodSolution.Value.Comp.Solution.Contents;

                    // check reagents in bloodstream
                    foreach (var (reagentId, quantity) in contents)
                    {
                        if (quantity <= FixedPoint2.Zero)
                            continue;

                        // check effects
                        if (!_prototypeManager.TryIndex<ReagentPrototype>(reagentId.Prototype, out var reagentProto))
                            continue;

                        if (reagentProto.Metabolisms == null || !reagentProto.Metabolisms.Metabolisms.TryGetValue("Bloodstream", out var metabolism))
                            continue;

                        var isAdrenaline = metabolism.Effects.Any(effect => effect is ModifyStatusEffect modify &&
                            modify.EffectProto.Id == "StatusEffectAdrenaline");

                        // if this reagent grants adrenaline, consume it and roll for revival
                        if (!isAdrenaline)
                            continue;

                        hasAdrenaline = true;

                        // removes the adrenaline cost amount
                        _solutionContainer.RemoveReagent(bloodSolution.Value, reagentId, FixedPoint2.New(_adrenalineCostPerShock));

                        break;
                    }
                }
            }

            var canRevive = true;
            if (_mobState.IsDead(target, targetMobState))
            {
                canRevive = false;

                if (hasAdrenaline)
                {
                    // server-only roll to prevent client mispredicting a successful revival
                    canRevive = _net.IsServer && _random.Prob(_reviveChance);
                }
                else
                {
                    // if they have no adrenaline reagent, popup
                    _popup.PopupClient(Loc.GetString("defibrillator-no-adrenaline"), target, user);
                }
            }

            // adrenaline zap heals 25 asphyx
            if (hasAdrenaline)
            {
                var asphyxHeal = new DamageSpecifier();
                asphyxHeal.DamageDict.Add("Asphyxiation", FixedPoint2.New(-25));
                _damageable.TryChangeDamage(target, asphyxHeal, true, origin: user);
            }
            // funky end

            if (canRevive && // funky
                TryComp<MobThresholdsComponent>(target, out var targetThresholds) &&
                _mobThreshold.TryGetThresholdForState(target, MobState.Dead, out var threshold, targetThresholds) &&
                _damageable.GetTotalDamage(target) < threshold) //is their current health above their death threshold
            {
                _mobState.ChangeMobState(target, MobState.Critical, targetMobState, user); //if so revive them
                failedRevive = false;
            }

            if (_mind.TryGetMind(target, out var mindUid, out var mindComp) &&
                _player.TryGetSessionById(mindComp.UserId, out var playerSession))
            {
                // notify them they're being revived.
                if (mindComp.CurrentEntity != target)
                    OpenReturnToBodyEui((mindUid, mindComp), playerSession);
            }
            else
            {
                if (HasComp<MindContainerComponent>(target))
                    _chat.TrySendInGameICMessage(ent.Owner, Loc.GetString("defibrillator-no-mind"), InGameICChatType.Speak, true); //target can host a mind but doesn't
                else
                    _chat.TrySendInGameICMessage(ent.Owner, Loc.GetString("defibrillator-not-living"), InGameICChatType.Speak, true); //target couldn't have hosted a mind
            }
        }

        _electrocution.TryDoElectrocution(
            target,
            ent.Owner,
            ent.Comp.ZapDamage,
            ent.Comp.WritheDuration,
            true,
            ignoreInsulation: isOriginal
        );

        return failedRevive;
    }

    // TODO: SharedEuiManager so that we can just directly open the eui from shared.
    protected virtual void OpenReturnToBodyEui(Entity<MindComponent> mind, ICommonSession session) { }
}
