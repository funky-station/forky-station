using Content.Server.Popups;
using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared._Funkystation.Unions;
using Content.Shared.Access.Systems;
using Content.Shared.Humanoid;
using Content.Shared.Interaction;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.Unions;

public sealed partial class UnionClipboardSystem : EntitySystem
{
    [Dependency] private UnionSelectorSystem _unionSelector = default!;
    [Dependency] private SharedIdCardSystem _idCard = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    private const int MaxNoteTitleLength = 64;
    private const int MaxNoteTextLength = 512;
    // how close a prospective member needs to be to a leader to be registered
    private const float RegisterLookupRange = 3f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UnionClipboardComponent, BoundUIOpenedEvent>(OnBoundUiOpened);
        SubscribeLocalEvent<UnionClipboardComponent, InteractUsingEvent>(OnInteractUsing);
        Subs.BuiEvents<UnionClipboardComponent>(UnionClipboardUiKey.Key,
            subs =>
            {
                subs.Event<UnionClipboardRemoveMemberMessage>(OnRemoveMember);
                subs.Event<UnionClipboardAddNoteMessage>(OnAddNote);
            });
    }

    private void OnInteractUsing(EntityUid uid, UnionClipboardComponent component, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!_idCard.TryGetIdCard(args.Used, out var idCard) || idCard.Comp.FullName is not { } cardName)
            return;

        if (!_unionSelector.TryGetUnionForGrouping(component.GroupingId, out var union) || union == null)
            return;

        if (union.Leader != args.User)
            return;
        
        // look at comment for this func to see why we do this in the first place
        var target = FindNearbyMatch(args.User, cardName);
        if (target == null)
        {
            _popup.PopupEntity(Loc.GetString("union-clipboard-no-match"), args.User, args.User);
            return;
        }

        if (!_unionSelector.TryRegisterMember(union, target.Value))
        {
            _popup.PopupEntity(Loc.GetString("union-clipboard-already-registered"), args.User, args.User);
            return;
        }

        _popup.PopupEntity(Loc.GetString("union-clipboard-registered", ("name", cardName)), args.User, args.User);
        UpdateUi((uid, component));
        args.Handled = true;
    }
    
    // we do this because we need to store the right entity uid, since thats how we keep track of them
    // in a single union unit
    // IM FAIRLY CERTAIN THIS WILL WORK FINE?
    private EntityUid? FindNearbyMatch(EntityUid user, string cardName)
    {
        var coords = _transform.GetMapCoordinates(user);
        foreach (var candidate in _lookup.GetEntitiesInRange<HumanoidProfileComponent>(coords, RegisterLookupRange))
        {
            if (string.Equals(Name(candidate), cardName, StringComparison.OrdinalIgnoreCase))
                return candidate;
        }

        return null;
    }

    private void OnBoundUiOpened(Entity<UnionClipboardComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnRemoveMember(Entity<UnionClipboardComponent> ent, ref UnionClipboardRemoveMemberMessage args)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader != args.Actor)
            return;

        var target = GetEntity(args.Target);
        if (!_unionSelector.RemoveMember(union, target))
            return;

        UpdateUi(ent);
    }

    private void OnAddNote(Entity<UnionClipboardComponent> ent, ref UnionClipboardAddNoteMessage args)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader != args.Actor)
            return;

        var target = GetEntity(args.Target);
        if (!union.Members.TryGetValue(target, out var info))
            return;

        var title = args.Title.Trim();
        var text = args.Text.Trim();
        if (title.Length == 0 || text.Length == 0)
            return;

        if (title.Length > MaxNoteTitleLength)
            title = title[..MaxNoteTitleLength];

        if (text.Length > MaxNoteTextLength)
            text = text[..MaxNoteTextLength];

        info.Notes.Add(new UnionMemberNote
        {
            Title = title,
            Text = text,
            Author = Name(args.Actor),
            Time = _timing.CurTime,
        });

        UpdateUi(ent);
    }

    private void UpdateUi(Entity<UnionClipboardComponent> ent)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        var members = new List<UnionClipboardMemberEntry>();
        foreach (var (memberUid, info) in union.Members)
        {
            var name = Name(memberUid);
            var jobTitle = string.Empty;

            if (_idCard.TryFindIdCard(memberUid, out var idCard))
            {
                name = idCard.Comp.FullName ?? name;
                jobTitle = idCard.Comp.LocalizedJobTitle;
            }

            var notes = new List<UnionClipboardNoteEntry>();
            foreach (var note in info.Notes)
            {
                notes.Add(new UnionClipboardNoteEntry(note.Title, note.Text, note.Author, note.Time.ToString(@"hh\:mm\:ss")));
            }

            members.Add(new UnionClipboardMemberEntry(GetNetEntity(memberUid), name, jobTitle!, memberUid == union.Leader, notes));
        }

        _ui.SetUiState(ent.Owner, UnionClipboardUiKey.Key, new UnionClipboardBoundUserInterfaceState(union.Name, members));
    }
}
