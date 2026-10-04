using System.Linq;
using Content.Shared._Funkystation.Patdown.Components;
using Content.Shared.DoAfter;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.Patdown;

/// <summary>
/// verb + doAfter for patting someone down
/// </summary>
public sealed partial class SharedPatdownSystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] private InventorySystem _inventory = null!;
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private TagSystem _tag = null!;
    [Dependency] private INetManager _net = null!;

    private static readonly ProtoId<TagPrototype> ConcealedTag = "PatdownConcealed";
    private static readonly SoundSpecifier RustleSound = new SoundCollectionSpecifier("storageRustle");

    private readonly Dictionary<EntityUid, PatdownSearchState> _activeSearches = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PatdownableComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
        SubscribeLocalEvent<PatdownableComponent, PatdownDoAfterEvent>(OnPatdownDoAfter);
    }

    private void OnGetVerbs(Entity<PatdownableComponent> target, ref GetVerbsEvent<InteractionVerb> args)
    {
        if (args.Hands == null || !args.CanAccess || !args.CanInteract || args.Target == args.User)
            return;

        if (_activeSearches.ContainsKey(target))
            return;

        var user = args.User;
        InteractionVerb verb = new()
        {
            Text = Loc.GetString("patdown-verb-text"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/_Funkystation/Interface/VerbIcons/patdown.png")),
            Act = () => TryStartPatdown(user, target, target.Comp),
        };

        args.Verbs.Add(verb);
    }

    public void TryStartPatdown(EntityUid user, EntityUid target, PatdownableComponent component)
    {
        if (_activeSearches.ContainsKey(target))
            return;

        // red alert popup
        _popup.PopupEntity(
            Loc.GetString("patdown-alert-target", ("user", Identity.Entity(user, EntityManager))),
            target,
            target,
            PopupType.LargeCaution);

        if (_net.IsServer)
            _activeSearches[target] = BuildSearchState(user, target, component.Delay);

        var doAfterArgs = new DoAfterArgs(EntityManager, user, component.Delay, new PatdownDoAfterEvent(), target, target)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            DistanceThreshold = SharedInteractionSystem.InteractionRange,
            NeedHand = true,
            BlockDuplicate = true,
            DuplicateCondition = DuplicateConditions.SameTarget,
        };

        if (!_doAfter.TryStartDoAfter(doAfterArgs))
            _activeSearches.Remove(target);
    }

    // builds the list of popups to be spaced across the duration of the doafter
    // items tagged PatdownConcealed, and anything inside clothing that's itself concealed, never show up
    private PatdownSearchState BuildSearchState(EntityUid user, EntityUid target, TimeSpan delay)
    {
        var state = new PatdownSearchState(user);

        if (!_inventory.TryGetSlots(target, out var slotDefinitions))
            return state;

        var validClothing = new List<EntityUid>();

        foreach (var slotDef in slotDefinitions)
        {
            if (!_inventory.TryGetSlotEntity(target, slotDef.Name, out var worn))
                continue;

            if (_tag.HasTag(worn.Value, ConcealedTag))
                continue;

            if (!TryComp<StorageComponent>(worn.Value, out var storage))
                continue;

            var hasVisibleItem = storage.StoredItems.Keys.Any(item => !_tag.HasTag(item, ConcealedTag));

            if (hasVisibleItem)
                validClothing.Add(worn.Value);
        }

        if (validClothing.Count <= 0)
            return state;

        var interval = delay / (validClothing.Count + 1);
        var nextReveal = _timing.CurTime + interval;

        foreach (var clothing in validClothing)
        {
            state.Pending.Add((nextReveal, clothing));
            nextReveal += interval;
        }

        return state;
    }

    private void OnPatdownDoAfter(Entity<PatdownableComponent> target, ref PatdownDoAfterEvent args)
    {
        _activeSearches.Remove(target.Owner);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_net.IsServer || _activeSearches.Count == 0)
            return;

        var now = _timing.CurTime;

        foreach (var (target, state) in _activeSearches)
        {
            for (var i = state.Pending.Count - 1; i >= 0; i--)
            {
                var (revealAt, clothing) = state.Pending[i];
                if (now < revealAt)
                    continue;

                _popup.PopupEntity(
                    Loc.GetString("patdown-item-found", ("clothing", clothing), ("target", target)),
                    target,
                    state.Searcher,
                    PopupType.Medium);

                _audio.PlayPvs(RustleSound, target);

                state.Pending.RemoveAt(i);
            }
        }
    }

    private sealed class PatdownSearchState(EntityUid searcher)
    {
        public readonly EntityUid Searcher = searcher;
        public readonly List<(TimeSpan RevealAt, EntityUid Clothing)> Pending = new();
    }
}
