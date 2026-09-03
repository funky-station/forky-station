using Content.Shared.Containers.ItemSlots;
using Content.Shared.DragDrop;
using Content.Shared.Mobs.Components;
using Robust.Shared.Containers;

namespace Content.Shared._Funkystation.LaundryCart;

public abstract partial class SharedLaundryCartSystem : EntitySystem
{
    [Dependency] protected SharedContainerSystem Container = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LaundryCartComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<LaundryCartComponent, CanDropTargetEvent>(OnCanDrop);
    }

    private void OnInit(Entity<LaundryCartComponent> cart, ref ComponentInit args)
    {
        cart.Comp.HiddenContainer = Container.EnsureContainer<Container>(cart.Owner, cart.Comp.HiddenContainerId);
    }

    private void OnCanDrop(Entity<LaundryCartComponent> cart, ref CanDropTargetEvent args)
    {
        if (args.Handled)
            return;

        if (!HasComp<MobStateComponent>(args.Dragged))
            return;

        if (!AllBagsFull(cart))
            return;

        if (cart.Comp.HiddenContainer.ContainedEntities.Count >= cart.Comp.MaxOccupants)
            return;

        args.CanDrop = true;
        args.Handled = true;
    }

    protected bool AllBagsFull(Entity<LaundryCartComponent> cart)
    {
        if (!TryComp<ItemSlotsComponent>(cart, out var slots))
            return false;

        var filled = 0;
        foreach (var (id, slot) in slots.Slots)
        {
            if (id.StartsWith("bag_slot") && slot.HasItem)
                filled++;
        }

        return filled >= cart.Comp.RequiredBags;
    }
}
