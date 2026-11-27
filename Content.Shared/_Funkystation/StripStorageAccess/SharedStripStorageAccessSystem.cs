using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Utility;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Strip.Components;

namespace Content.Shared._Funkystation.StripStorageAccess;

/// <summary>
/// lets someone open a person's worn storage from the strip menu via interact
/// </summary>
public sealed partial class SharedStripStorageAccessSystem : EntitySystem
{
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private  SharedStorageSystem _storage = null!;
    [Dependency] private InventorySystem _inventory = null!;
    [Dependency] private SharedHandsSystem _hands = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedContainerSystem _container = null!;
    [Dependency] private SharedInteractionSystem _interaction = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StrippableComponent, StrippingOpenStorageButtonPressed>(OnOpenStorageButtonPressed);

        SubscribeLocalEvent<StorageComponent, StripStorageOpenDoAfterEvent>(OnOpenDoAfter);
        SubscribeLocalEvent<StorageComponent, StripStorageRemoveDoAfterEvent>(OnRemoveDoAfter);
        SubscribeLocalEvent<StorageComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
    }

    private void OnOpenStorageButtonPressed(Entity<StrippableComponent> strippable, ref StrippingOpenStorageButtonPressed args)
    {

        if (args.Actor is not { Valid: true } user || !TryComp<HandsComponent>(user, out _))
        {
            return;
        }

        EntityUid? slotEntity;
        if (args.IsHand)
        {
            slotEntity = _hands.GetHeldItem(strippable.Owner, args.Slot);
        }
        else
        {
            if (!TryComp<InventoryComponent>(strippable, out var inventory) ||
                !_inventory.TryGetSlotEntity(strippable, args.Slot, out var held, inventory))
            {
                return;
            }

            slotEntity = held;
        }


        if (slotEntity is not { } storageUid || !HasComp<StorageComponent>(storageUid))
        {
            return;
        }

        StartOpenStorageDoAfter(user, storageUid, strippable.Owner);
    }

    private void OnGetVerbs(Entity<StorageComponent> target, ref GetVerbsEvent<InteractionVerb> args)
    {
        if (args.Hands == null || !args.CanInteract || args.Target == args.User)
            return;

        if (!_container.TryGetContainingContainer(target.Owner, out var container) || container.Owner == args.User)
            return;

        if (!HasComp<InventoryComponent>(container.Owner) && !HasComp<HandsComponent>(container.Owner))
            return;

        var user = args.User;
        var holder = container.Owner;
        var storageUid = target.Owner;

        if (!_interaction.InRangeUnobstructed(user, holder))
            return;

        InteractionVerb verb = new()
        {
            Text = Loc.GetString("strip-storage-access-verb-text"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/open.svg.192dpi.png")),
            Act = () => StartOpenStorageDoAfter(user, storageUid, holder),
        };

        args.Verbs.Add(verb);
    }

    private void StartOpenStorageDoAfter(EntityUid user, EntityUid storageUid, EntityUid holder)
    {
        var doAfterArgs = new DoAfterArgs(EntityManager, user, TimeSpan.FromSeconds(5), new StripStorageOpenDoAfterEvent(), storageUid, holder)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            DistanceThreshold = SharedInteractionSystem.InteractionRange,
            NeedHand = true,
            BlockDuplicate = true,
            DuplicateCondition = DuplicateConditions.SameTarget,
        };

        var started = _doAfter.TryStartDoAfter(doAfterArgs);

        if (!started)
            return;

        _audio.PlayPredicted(new SoundCollectionSpecifier("storageRustle"), storageUid, user);

        _popup.PopupEntity(
            Loc.GetString("strip-storage-access-alert-target",
                ("user", Identity.Entity(user, EntityManager)),
                ("item", storageUid)),
            holder,
            holder,
            PopupType.Medium);
    }

    private void OnOpenDoAfter(Entity<StorageComponent> storage, ref StripStorageOpenDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        args.Handled = true;

        // bypass interaction checks because the item is inside a "container"
        var alreadyHas = HasComp<BypassInteractionChecksComponent>(args.User);
        if (!alreadyHas)
            AddComp<BypassInteractionChecksComponent>(args.User);

        _storage.OpenStorageUI(storage, args.User);

        if (!alreadyHas)
            RemComp<BypassInteractionChecksComponent>(args.User);
    }

    private void OnRemoveDoAfter(Entity<StorageComponent> storage, ref StripStorageRemoveDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (args.Used is not { } item ||
            !TryComp<HandsComponent>(args.User, out var hands) ||
            !TryComp<ItemComponent>(item, out var itemComp))
            return;

        args.Handled = true;

        var alreadyHas = HasComp<BypassInteractionChecksComponent>(args.User);
        if (!alreadyHas)
            AddComp<BypassInteractionChecksComponent>(args.User);

        _storage.DoInteractWithStoredItem((args.User, hands), storage, (item, itemComp));

        if (!alreadyHas)
            RemComp<BypassInteractionChecksComponent>(args.User);
    }
}
