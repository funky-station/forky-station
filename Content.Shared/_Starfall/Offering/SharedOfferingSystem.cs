using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Strip.Components;
using Robust.Shared.Timing;

namespace Content.Shared._Starfall.Offering;

/// <summary>
/// Handles consensual hand-to-hand item transfers.
/// </summary>
public sealed partial class SharedOfferingSystem : EntitySystem
{
    [Dependency] private SharedHandsSystem _hands = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private IGameTiming _timing = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StripHandInsertAttemptEvent>(OnStripHandInsertAttempt);
        SubscribeLocalEvent<StrippableComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<OfferedItemComponent, StripHandRemoveAttemptEvent>(OnStripHandRemoveAttempt);
        SubscribeLocalEvent<OfferedItemComponent, EntParentChangedMessage>(OnParentChanged);
    }

    private void OnStripHandInsertAttempt(ref StripHandInsertAttemptEvent args)
    {
        if (!CanOffer(args.User, args.Recipient, args.Item))
            return;

        Offer(args.User, args.Recipient, args.Item, EnsureComp<OfferedItemComponent>(args.Item));
    }

    private void OnInteractHand(Entity<StrippableComponent> offerer, ref InteractHandEvent args)
    {
        if (args.Handled || args.User == offerer.Owner || !TryComp<HandsComponent>(args.User, out var recipientHands) || _hands.GetActiveItem((args.User, recipientHands)) != null || !TryComp<HandsComponent>(offerer, out var offererHands))
            return;

        foreach (var handName in offererHands.Hands.Keys)
        {
            if (!_hands.TryGetHeldItem((offerer.Owner, offererHands), handName, out var item) || !TryComp<OfferedItemComponent>(item.Value, out var offered) || offered.Offerer != offerer.Owner || offered.Recipient != args.User)
                continue;

            args.Handled = TryAccept(args.User, offerer.Owner, item.Value, handName);
            return;
        }
    }

    private void OnStripHandRemoveAttempt(Entity<OfferedItemComponent> item, ref StripHandRemoveAttemptEvent args)
    {
        if (item.Comp.Offerer != args.Holder || item.Comp.Recipient != args.User)
            return;

        TryAccept(args.User, args.Holder, item.Owner, args.HandName);
        args.Handled = true;
    }

    private void Offer(EntityUid offerer, EntityUid recipient, EntityUid item, OfferedItemComponent offered)
    {
        offered.Offerer = offerer;
        offered.Recipient = recipient;
        Dirty(item, offered);
    }

    private bool CanOffer(EntityUid offerer, EntityUid recipient, EntityUid item)
    {
        return offerer.IsValid() && recipient.IsValid() && item.IsValid() &&
               offerer != recipient && item != offerer && item != recipient &&
               !TerminatingOrDeleted(offerer) && !TerminatingOrDeleted(recipient) && !TerminatingOrDeleted(item) &&
               TryComp<HandsComponent>(offerer, out var offererHands) &&
               TryComp<HandsComponent>(recipient, out _) &&
               _hands.GetActiveItem((offerer, offererHands)) == item;
    }

    private bool TryAccept(EntityUid recipient, EntityUid offerer, EntityUid item, string handName)
    {
        // okay to explain myself:
        // Make sure the recipient, offerer, and item are valid entity references.
        // Prevent offering to yourself or treating either participant as the offered item.
        // Make sure none of the entities are currently being deleted.
        // Confirm the item still has an active offer.
        // Confirm this is the intended recipient accepting from the original offerer.
        // Confirm the offerer is still holding this exact item in the expected hand.
        if (!recipient.IsValid() || !offerer.IsValid() || !item.IsValid() ||
            recipient == offerer || item == recipient || item == offerer ||
            TerminatingOrDeleted(recipient) || TerminatingOrDeleted(offerer) || TerminatingOrDeleted(item) ||
            !TryComp<OfferedItemComponent>(item, out var offered) ||
            offered.Recipient != recipient || offered.Offerer != offerer ||
            !TryComp<HandsComponent>(offerer, out var offererHands) ||
            _hands.GetHeldItem((offerer, offererHands), handName) != item)
        {
            PopupFailure(recipient);
            return false;
        }

        // Remove the offer immediately so duplicate events in this tick cannot accept it twice.
        RemComp<OfferedItemComponent>(item);
        if (!_hands.TryDrop((offerer, offererHands), item, checkActionBlocker: false))
        {
            PopupFailure(recipient);
            return false;
        }

        if (!_hands.TryPickupAnyHand(recipient, item))
        {
            // Pickup hooks can still reject the transfer after the initial validation. Put it back if possible.
            _hands.TryPickup(offerer, item, handName, checkActionBlocker: false, handsComp: offererHands);
            PopupFailure(recipient);
            return false;
        }

        // Notify the recipient that they accepted the offer. We do this first time predicted to avoid duplicate popups.
        if (_timing is { IsFirstTimePredicted: true, InPrediction: true })
            _popup.PopupEntity(Loc.GetString("offering-system-accepted-self", ("item", item)), recipient, recipient);

        return true;
    }

    private void PopupFailure(EntityUid recipient)
    {
        if (_timing is { IsFirstTimePredicted: true, InPrediction: true } && recipient.IsValid() && !TerminatingOrDeleted(recipient))
            _popup.PopupEntity(Loc.GetString("offering-system-cannot-accept"), recipient, recipient);
    }

    private void OnParentChanged(Entity<OfferedItemComponent> item, ref EntParentChangedMessage args)
    {
        // An offer is only valid while the original offerer continuously holds the item.
        if (!item.Comp.Offerer.IsValid() || TerminatingOrDeleted(item.Comp.Offerer) ||
            !TryComp<HandsComponent>(item.Comp.Offerer, out var hands) ||
            !_hands.IsHolding((item.Comp.Offerer, hands), item.Owner))
            RemCompDeferred<OfferedItemComponent>(item);
    }
}
