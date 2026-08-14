using Content.Shared.Storage.Components;
using Robust.Shared.Containers;

namespace Content.Shared._Funkystation.Viewcone;

public sealed class ViewconeStorageBlindSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EntityStorageComponent, StorageAfterCloseEvent>(OnStorageClosed);
        SubscribeLocalEvent<EntityStorageComponent, StorageAfterOpenEvent>(OnStorageOpened);
        SubscribeLocalEvent<EntityStorageComponent, EntInsertedIntoContainerMessage>(OnInserted);
        SubscribeLocalEvent<EntityStorageComponent, EntRemovedFromContainerMessage>(OnRemoved);
    }

    // mark as blind when closed
    private void OnStorageClosed(Entity<EntityStorageComponent> ent, ref StorageAfterCloseEvent args)
    {
        if (ent.Comp.Contents is not { } contents)
            return;

        foreach (var contained in contents.ContainedEntities)
        {
            EnsureComp<ViewconeStorageBlindComponent>(contained);
        }
    }

    // unmark
    private void OnStorageOpened(Entity<EntityStorageComponent> ent, ref StorageAfterOpenEvent args)
    {
        if (ent.Comp.Contents is not { } contents)
            return;

        foreach (var contained in contents.ContainedEntities)
        {
            RemComp<ViewconeStorageBlindComponent>(contained);
        }
    }

    // in case something is somehow inserted while still closed
    private void OnInserted(Entity<EntityStorageComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        if (ent.Comp.Contents is not { } contents || args.Container.ID != contents.ID)
            return;

        if (!ent.Comp.Open)
            EnsureComp<ViewconeStorageBlindComponent>(args.Entity);
    }

    // and removes marker when something leaves storage
    private void OnRemoved(Entity<EntityStorageComponent> ent, ref EntRemovedFromContainerMessage args)
    {
        if (ent.Comp.Contents is not { } contents || args.Container.ID != contents.ID)
            return;

        RemComp<ViewconeStorageBlindComponent>(args.Entity);
    }
}
