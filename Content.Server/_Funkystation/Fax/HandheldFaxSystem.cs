using Content.Shared._Funkystation.Fax;
using Content.Shared.Fax.Components;
using Content.Shared.Gibbing;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Mobs.Components;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Containers;

namespace Content.Server._Funkystation.Fax;

/// <summary>
/// _Funkystation: When any fax machine is inside a storage container like a backpack,
/// redirects printed papers to that storage instead of dropping them at the fax's coordinates.
/// Falls back to the hands of whoever is carrying the storage, then drops on the ground.
/// Also handles gibs produced by faxecuting a mob while the fax is in storage.
/// </summary>
public sealed partial class HandheldFaxSystem : EntitySystem
{
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private SharedStorageSystem _storage = default!;
    [Dependency] private SharedHandsSystem _hands = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FaxMachineComponent, FaxPaperPrintedEvent>(OnFaxPaperPrinted);

        // FaxecuteSystem raises this on the fax uid just before applying damage.
        SubscribeLocalEvent<FaxMachineComponent, FaxecuteFiringEvent>(OnFaxecuteFiring);
        SubscribeLocalEvent<MobStateComponent, GibbedBeforeDeletionEvent>(OnMobGibbed);
    }

    // Mobs currently being faxecuted that we care about
    private readonly HashSet<EntityUid> _pendingFaxecute = new();

    // Redirect any faxes printed/received into the storage the fax is inside of (if it is)
    // If the storage is full, resort to the hands of whoever is carrying the storage,
    // otherwise drop on the ground if that's not possible.
    private void OnFaxPaperPrinted(EntityUid faxUid, FaxMachineComponent component, ref FaxPaperPrintedEvent args)
    {
        // Only redirect if the fax is actually inside a container.
        if (!_container.TryGetContainingContainer(faxUid, out var parentContainer))
            return;

        var parentEntity = parentContainer.Owner;

        // Try inserting into whatever storage the fax is inside of.
        if (TryComp<StorageComponent>(parentEntity, out var storageComp) &&
            _storage.Insert(parentEntity, args.Paper, out _, storageComp: storageComp, playSound: true))
        {
            args.Handled = true;
            return;
        }

        // Storage full or otherwise cant put it in there, try the hands of whoever holds the parent.
        if (_container.TryGetContainingContainer(parentEntity, out var grandparentContainer) &&
            _hands.TryPickupAnyHand(grandparentContainer.Owner, args.Paper, checkActionBlocker: false))
        {
            args.Handled = true;
            return;
        }

        // If none of the above conditions can be met, drop on the ground near the fax machine.
        _transform.AttachToGridOrMap(args.Paper);
        args.Handled = true;
    }

    // Faxecution handling
    private void OnFaxecuteFiring(EntityUid faxUid, FaxMachineComponent faxComp, ref FaxecuteFiringEvent args)
    {
        if (!_container.TryGetContainingContainer(faxUid, out _))
            return;

        _pendingFaxecute.Add(args.Mob);
    }

    private void OnMobGibbed(EntityUid mobUid, MobStateComponent comp, ref GibbedBeforeDeletionEvent args)
    {
        if (!_pendingFaxecute.Remove(mobUid))
            return;

        if (!_container.TryGetContainingContainer(mobUid, out var innerContainer))
            return;

        var faxUid = innerContainer.Owner;
        if (!TryComp<FaxMachineComponent>(faxUid, out var faxComp))
            return;

        if (innerContainer.ID != faxComp.PaperSlot.ID)
            return;

        foreach (var giblet in args.Giblets)
        {
            if (!Exists(giblet))
                continue;

            _transform.AttachToGridOrMap(giblet);
        }
    }
}
