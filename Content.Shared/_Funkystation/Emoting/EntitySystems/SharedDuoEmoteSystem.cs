using Content.Shared._Funkystation.Emoting.Components;
using Content.Shared._Funkystation.Emoting.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._Funkystation.Emoting.EntitySystems;

public abstract partial class SharedDuoEmoteSystem : EntitySystem
{
    [Dependency] private INetManager _net = null!;
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] private RotateToFaceSystem _rotate = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] protected  SharedInteractionSystem _interaction = null!;
    [Dependency] private SharedTransformSystem _transform = null!;
    [Dependency] protected IPrototypeManager _prototype = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DuoEmoteComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<DuoEmoteComponent, MoveInputEvent>(OnMove);
    }

    private void OnInteractHand(Entity<DuoEmoteComponent> ent, ref InteractHandEvent args)
    {
        if (args.Handled)
            return;

        var user = args.User;

        if (user == ent.Owner)
            return;

        if (!TryComp<DuoEmoteComponent>(user, out var userComp))
            return;

        // ent is the initiator (raised their hand), user is the partner clicking back
        if (!ent.Comp.Active || userComp.Active)
            return;

        if (ent.Comp.Target != user)
            return;

        if (!_interaction.InRangeUnobstructed(user, ent.Owner, ent.Comp.InteractRange))
        {
            _popup.PopupClient(Loc.GetString("duo-emote-get-closer"), user, user);
            return;
        }

        args.Handled = true;
        PerformEmote(ent, (user, userComp));
    }

    private void OnMove(Entity<DuoEmoteComponent> ent, ref MoveInputEvent args)
    {
        if (!args.HasDirectionalMovement)
            return;

        if (!ent.Comp.Active)
            return;

        CancelDuoEmote(ent);
    }

    protected void AttemptDuoEmote(Entity<DuoEmoteComponent> initiator, Entity<DuoEmoteComponent> target, ProtoId<DuoEmotePrototype> emoteId)
    {
        if (initiator.Comp.Active || target.Comp.Active)
            return;

        if (!_prototype.TryIndex(emoteId, out var proto))
            return;

        if (proto.RequireTail)
        {
            if (!HasComp<TailDuoEmoteComponent>(initiator.Owner))
                return;

            if (!HasComp<TailDuoEmoteComponent>(target.Owner))
            {
                var msg = Loc.GetString("duo-emote-no-tail", ("target", target.Owner));
                _popup.PopupEntity(msg, initiator.Owner, initiator.Owner, PopupType.SmallCaution);
                return;
            }
        }

        initiator.Comp.Active = true;
        initiator.Comp.Target = target.Owner;
        initiator.Comp.EmoteId = emoteId;
        initiator.Comp.LeaveHangingAt = _timing.CurTime + initiator.Comp.LeftHangingDelay;

        if (_net.IsServer)
        {
            var effect = SpawnAttachedTo("FunkyEffectDuoEmoteBase", initiator.Owner.ToCoordinates());
            var visuals = EnsureComp<DuoEmoteVisualsComponent>(effect);
            visuals.EmoteId = emoteId;
            Dirty(effect, visuals);
            initiator.Comp.SpawnedEffect = effect;
        }

        var popupSelf = Loc.GetString(proto.AttemptSelf, ("target", target.Owner));
        var popupOthers = Loc.GetString(proto.AttemptOthers, ("ent", initiator.Owner), ("target", target.Owner));

        _popup.PopupPredicted(popupSelf, popupOthers, initiator.Owner, initiator.Owner, PopupType.Medium);
        Dirty(initiator);
    }

    private void PerformEmote(Entity<DuoEmoteComponent> initiator, Entity<DuoEmoteComponent> partner)
    {
        var initiatorUid = initiator.Owner;
        var partnerUid = partner.Owner;
        var emoteId = initiator.Comp.EmoteId;

        if (emoteId == null || !_prototype.TryIndex(emoteId.Value, out var proto))
        {
            CancelDuoEmote(initiator);
            CancelDuoEmote(partner);
            return;
        }

        var sound = proto.Sound;

        // Popup seen by the initiator
        var popupInitiator = Loc.GetString(proto.PerformSelf, ("target", partnerUid));

        // Popup seen by the partner
        var popupPartner = Loc.GetString(proto.PerformSelf, ("target", initiatorUid));

        // Third-person popup for bystanders
        var popupOthers = Loc.GetString(proto.PerformOthers, ("ent", initiatorUid), ("target", partnerUid));

        // Both face each other
        _rotate.TryFaceCoordinates(initiatorUid, _transform.GetMapCoordinates(partnerUid).Position);
        _rotate.TryFaceCoordinates(partnerUid, _transform.GetMapCoordinates(initiatorUid).Position);

        if (_net.IsServer)
        {
            // Send popups directly to each participant via ActorComponent
            if (TryComp<ActorComponent>(initiatorUid, out var initiatorActor))
                _popup.PopupEntity(popupInitiator, initiatorUid, Filter.SinglePlayer(initiatorActor.PlayerSession), false, PopupType.Medium);
            if (TryComp<ActorComponent>(partnerUid, out var partnerActor))
                _popup.PopupEntity(popupPartner, partnerUid, Filter.SinglePlayer(partnerActor.PlayerSession), false, PopupType.Medium);

            // Bystander popup
            var others = Filter.PvsExcept(initiatorUid).RemovePlayerByAttachedEntity(partnerUid);
            _popup.PopupEntity(popupOthers, initiatorUid, others, true);

            if (sound != null)
                _audio.PlayPvs(sound, initiatorUid);

            RaiseNetworkEvent(new DuoEmoteLungeEvent(GetNetEntity(initiatorUid), GetNetEntity(partnerUid), proto.Animation));
        }

        CancelDuoEmote(initiator);
        CancelDuoEmote(partner);
    }

    private void CancelDuoEmote(Entity<DuoEmoteComponent> ent)
    {
        ent.Comp.Active = false;
        ent.Comp.Target = null;
        ent.Comp.EmoteId = null;

        if (_net.IsServer && ent.Comp.SpawnedEffect != null)
            QueueDel(ent.Comp.SpawnedEffect);

        ent.Comp.SpawnedEffect = null;

        Dirty(ent);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_net.IsClient)
            return;

        var time = _timing.CurTime;
        var query = EntityQueryEnumerator<DuoEmoteComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            if (!comp.Active)
                continue;

            if (time < comp.LeaveHangingAt)
                continue;

            CancelDuoEmote((uid, comp));
            _popup.PopupEntity(Loc.GetString("duo-emote-left-hanging"), uid, uid, PopupType.SmallCaution);
        }
    }
}
