using System.Linq;
using System.Numerics;
using Content.Shared._Funkystation.LaundryCart;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Popups;
using Content.Shared.Damage.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Throwing;
using Content.Server.Chat.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Random;

namespace Content.Server._Funkystation.LaundryCart;

public sealed partial class LaundryCartSystem : SharedLaundryCartSystem
{
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private ChatSystem _chatSystem = null!;
    [Dependency] private ItemSlotsSystem _itemSlots = null!;
    [Dependency] private ThrowingSystem _throwing = null!;
    [Dependency] private IRobustRandom _random = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LaundryCartComponent, DragDropTargetEvent>(OnDragDrop);
        SubscribeLocalEvent<LaundryCartComponent, LaundryCartHideDoAfterEvent>(OnHideDoAfter);
        SubscribeLocalEvent<LaundryCartComponent, EntRemovedFromContainerMessage>(OnContainerModified);
        SubscribeLocalEvent<LaundryCartComponent, DamageDealtEvent>(OnDamageDealt);
        SubscribeLocalEvent<LaundryCartComponent, ContainerRelayMovementEntityEvent>(OnRelayMovement);
    }

    private void PopupToEntity(string? message, EntityUid uid, EntityUid recipient, PopupType type = PopupType.Small)
    {
        _popup.PopupEntity(message, uid, recipient, type);
    }

    private void OnDragDrop(Entity<LaundryCartComponent> cart, ref DragDropTargetEvent args)
    {
        if (args.Handled)
            return;

        if (!AllBagsFull(cart) || cart.Comp.HiddenContainer.ContainedEntities.Count >= cart.Comp.MaxOccupants)
        {
            PopupToEntity(Loc.GetString("laundry-cart-cant-hide"), cart.Owner, args.User);
            return;
        }

        args.Handled = true;

        var doAfterArgs = new DoAfterArgs(EntityManager,
            args.User,
            cart.Comp.HideDelay,
            new LaundryCartHideDoAfterEvent(),
            cart.Owner,
            target: args.Dragged,
            used: cart.Owner)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = false,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnHideDoAfter(Entity<LaundryCartComponent> cart, ref LaundryCartHideDoAfterEvent args)
    {
        if (args.Cancelled || args.Target is not { } target)
            return;

        if (!AllBagsFull(cart) || cart.Comp.HiddenContainer.ContainedEntities.Count >= cart.Comp.MaxOccupants)
            return;

        Container.Insert(target, cart.Comp.HiddenContainer);
        PopupToEntity(Loc.GetString("laundry-cart-hide-success"), cart.Owner, target);
    }

    private void OnRelayMovement(Entity<LaundryCartComponent> cart, ref ContainerRelayMovementEntityEvent args)
    {
        if (!cart.Comp.HiddenContainer.Contains(args.Entity))
            return;

        PopupToEntity(Loc.GetString("laundry-cart-escape-attempt"), cart.Owner, args.Entity);

        ScatterBags(cart);
    }

    private void ScatterBags(Entity<LaundryCartComponent> cart)
    {
        if (!TryComp<ItemSlotsComponent>(cart, out var slots))
            return;

        foreach (var (id, slot) in slots.Slots)
        {
            if (!id.StartsWith("bag_slot") || !slot.HasItem)
                continue;

            if (!_itemSlots.TryEject(cart.Owner, slot, null, out var bag))
                continue;

            var angle = _random.NextFloat(0f, MathF.Tau);
            var direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * _random.NextFloat(0.3f, 0.8f);
            _throwing.TryThrow(bag.Value, direction, user: cart.Owner);
        }
    }

    private void OnContainerModified(Entity<LaundryCartComponent> cart, ref EntRemovedFromContainerMessage args)
    {
        // only care about the bag slots
        var containerId = args.Container.ID;
        if (!containerId.StartsWith("bag_slot"))
            return;

        if (AllBagsFull(cart))
            return;

        EjectAll(cart);
    }

    private void OnDamageDealt(Entity<LaundryCartComponent> cart, ref DamageDealtEvent args)
    {
        if (cart.Comp.HiddenContainer.ContainedEntities.Count == 0)
            return;

        foreach (var occupant in cart.Comp.HiddenContainer.ContainedEntities)
        {
            _chatSystem.TryEmoteWithChat(occupant, "Scream");
        }
    }

    private void EjectAll(Entity<LaundryCartComponent> cart)
    {
        foreach (var occupant in cart.Comp.HiddenContainer.ContainedEntities.ToArray())
        {
            Container.Remove(occupant, cart.Comp.HiddenContainer);
            PopupToEntity(Loc.GetString("laundry-cart-ejected"), cart.Owner, occupant, PopupType.Medium);
        }
    }
}
