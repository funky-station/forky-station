using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Shared._Funkystation.Shredder;
using Content.Shared._Funkystation.Shredder.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage.Components;
using Content.Shared.Interaction;
using Content.Shared.Paper;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.Shredder;

// for toggling the shredder on and off, shredding inserted paper, and depositing into the bin
public sealed partial class ShredderSystem : EntitySystem
{
    [Dependency] private SharedAppearanceSystem _appearance = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private ItemSlotsSystem _itemSlots = null!;
    [Dependency] private EntityWhitelistSystem _whitelist = null!;
    [Dependency] private PopupSystem _popup = null!;
    [Dependency] private SharedStorageSystem _storage = null!;

    private const string BinSlotId = "Bin";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShredderComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<ShredderComponent, EntInsertedIntoContainerMessage>(OnBinSlotChanged);
        SubscribeLocalEvent<ShredderComponent, EntRemovedFromContainerMessage>(OnBinSlotChanged);
        SubscribeLocalEvent<ShredderComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
        SubscribeLocalEvent<ShredderComponent, InteractUsingEvent>(OnInteractUsing);
    }

    private void OnComponentInit(EntityUid uid, ShredderComponent comp, ComponentInit args)
    {
        if (TryComp<ItemSlotsComponent>(uid, out var itemSlots) && _itemSlots.TryGetSlot(uid, BinSlotId, out var slot, itemSlots))
            comp.BinSlot = slot;

        if (!TryComp<ApcPowerReceiverComponent>(uid, out var receiver))
            return;
        receiver.PowerDisabled = !comp.Enabled;
        Dirty(uid, receiver);
    }

    private void OnBinSlotChanged(EntityUid uid, ShredderComponent comp, ContainerModifiedMessage args)
    {
        if (args.Container.ID != BinSlotId)
            return;

        var hasItem = TryComp<ItemSlotsComponent>(uid, out var itemSlots)
            && _itemSlots.TryGetSlot(uid, BinSlotId, out var slot, itemSlots)
            && slot.HasItem;

        _appearance.SetData(uid, ShredderVisuals.BinPresent, hasItem);
    }

    private void OnGetVerbs(EntityUid uid, ShredderComponent comp, GetVerbsEvent<InteractionVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        var enabled = comp.Enabled;

        args.Verbs.Add(new InteractionVerb
        {
            Text = Loc.GetString(enabled ? "shredder-verb-turn-off" : "shredder-verb-turn-on"),
            Act = () => SetEnabled(uid, comp, !enabled, args.User)
        });
    }

    private void SetEnabled(EntityUid uid, ShredderComponent comp, bool enabled, EntityUid user)
    {
        comp.Enabled = enabled;

        if (TryComp<ApcPowerReceiverComponent>(uid, out var receiver))
        {
            receiver.PowerDisabled = !enabled;
            Dirty(uid, receiver);
        }

        if (TryComp<DamageOnInteractComponent>(uid, out var damageInteract))
        {
            damageInteract.IsDamageActive = enabled;
            Dirty(uid, damageInteract);
        }

        _audio.PlayPvs(new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg"), uid);
        _popup.PopupEntity(Loc.GetString(enabled ? "shredder-on" : "shredder-off"), uid, user);

        if (!enabled)
        {
            comp.Shredding = false;
            _appearance.SetData(uid, ShredderVisuals.VisualState, ShredderVisualState.Idle);
        }
    }

    private void OnInteractUsing(EntityUid uid, ShredderComponent comp, InteractUsingEvent args)
    {
        if (args.Handled || !comp.Enabled || comp.Shredding)
            return;

        if (TryComp<ApcPowerReceiverComponent>(uid, out var receiver) && !receiver.Powered)
            return;

        var shreddable = comp.Whitelist != null
            ? _whitelist.IsValid(comp.Whitelist, args.Used)
            : HasComp<PaperComponent>(args.Used);

        if (!shreddable)
            return;

        args.Handled = true;
        comp.Shredding = true;

        _audio.PlayPvs(comp.ShredSound, uid);
        _appearance.SetData(uid, ShredderVisuals.VisualState, ShredderVisualState.Shredding);

        QueueDel(args.Used);

        Timer.Spawn(comp.ShredDelay,
            () =>
        {
            if (!Exists(uid) || Deleted(uid))
                return;

            comp.Shredding = false;
            _appearance.SetData(uid, ShredderVisuals.VisualState, ShredderVisualState.Idle);
            DepositShreddedPaper(uid, comp);
        });
    }

    private void DepositShreddedPaper(EntityUid uid, ShredderComponent comp)
    {
        var coords = Transform(uid).Coordinates;
        var paper = Spawn(comp.FloorPilePrototype, coords);

        if (comp.BinSlot.Item is { } binUid && TryComp<StorageComponent>(binUid, out _))
        {
            _storage.Insert(binUid, paper, out _);
        }
    }
}
