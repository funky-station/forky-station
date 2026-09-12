using Content.Shared.CartridgeLoader;
using Robust.Shared.Containers;

namespace Content.Shared._Funkystation.CartridgeLoader;

public sealed partial class AutoActivateCartridgeSystem : EntitySystem
{
    [Dependency] private CartridgeLoaderSystem _cartridgeLoader = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AutoActivateCartridgeComponent, EntInsertedIntoContainerMessage>(OnItemInserted);
        SubscribeLocalEvent<AutoActivateCartridgeComponent, BoundUIOpenedEvent>(OnUiOpened);
    }

    // activates the program the moment it's in the loader
    private void OnItemInserted(Entity<AutoActivateCartridgeComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        if (args.Container.ID != CartridgeLoaderComponent.RemovableContainerId
            && args.Container.ID != CartridgeLoaderComponent.UnremovableContainerId
            && args.Container.ID != CartridgeLoaderComponent.CartridgeSlotId)
            return;

        if (!TryComp<CartridgeLoaderComponent>(ent.Owner, out var loader))
            return;

        _cartridgeLoader.ActivateProgram((ent.Owner, loader), args.Entity);
    }

    private void OnUiOpened(Entity<AutoActivateCartridgeComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (!TryComp<CartridgeLoaderComponent>(ent.Owner, out var loader))
            return;

        _cartridgeLoader.UpdateUiState((ent.Owner, loader));
        _cartridgeLoader.RequestUiRefresh((ent.Owner, loader));
    }
}
