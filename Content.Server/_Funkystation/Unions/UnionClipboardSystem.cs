using System.Linq;
using Content.Server.Popups;
using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared._Funkystation.Unions;
using Content.Shared.Access.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Roles;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
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
    [Dependency] private SharedDoAfterSystem _doAfter = default!;

    private const int MaxNoteTitleLength = 64;
    private const int MaxNoteTextLength = 512;
    // how close a prospective member needs to be to a leader to be registered
    private const float RegisterLookupRange = 3f;
    private static readonly ProtoId<DepartmentPrototype> CommandDepartment = "Command";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UnionClipboardComponent, BoundUIOpenedEvent>(OnBoundUiOpened);
        SubscribeLocalEvent<UnionClipboardComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<UnionClipboardComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<UnionClipboardComponent, UnionClipboardClaimLeadershipDoAfterEvent>(OnClaimLeadershipDoAfter);
        Subs.BuiEvents<UnionClipboardComponent>(UnionClipboardUiKey.Key,
            subs =>
            {
                subs.Event<UnionClipboardRemoveMemberMessage>(OnRemoveMember);
                subs.Event<UnionClipboardAddNoteMessage>(OnAddNote);
                subs.Event<UnionClipboardBeginStewardMessage>(OnBeginSteward);
                subs.Event<UnionClipboardCancelStewardMessage>(OnCancelSteward);
                subs.Event<UnionClipboardAssignStewardMessage>(OnAssignSteward);
            });
        Subs.BuiEvents<UnionClipboardComponent>(UnionClipboardClaimUiKey.Key,
            subs => subs.Event<UnionClipboardClaimLeadershipConfirmMessage>(OnConfirmClaimLeadership));
    }

    private void OnUseInHand(Entity<UnionClipboardComponent> ent, ref UseInHandEvent args)
    {
        if (args.Handled)
            return;
        args.Handled = true;

        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader == args.User)
        {
            _ui.TryToggleUi(ent.Owner, UnionClipboardUiKey.Key, args.User);
            return;
        }

        if (!union.Members.ContainsKey(args.User))
        {
            _popup.PopupEntity(Loc.GetString("union-clipboard-not-a-member"), args.User, args.User);
            return;
        }

        _ui.OpenUi(ent.Owner, UnionClipboardClaimUiKey.Key, args.User);
    }

    private void OnConfirmClaimLeadership(Entity<UnionClipboardComponent> ent, ref UnionClipboardClaimLeadershipConfirmMessage args)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader == args.Actor || !union.Members.ContainsKey(args.Actor))
            return;

        _ui.CloseUi(ent.Owner, UnionClipboardClaimUiKey.Key, args.Actor);

        _popup.PopupEntity(Loc.GetString("union-clipboard-claim-leadership-prompt"), args.Actor, args.Actor);

        var doAfterArgs = new DoAfterArgs(EntityManager, args.Actor, ent.Comp.ClaimLeadershipDelay, new UnionClipboardClaimLeadershipDoAfterEvent(), ent.Owner, used: ent.Owner)
        {
            BreakOnMove = true,
            NeedHand = true,
        };
        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnClaimLeadershipDoAfter(Entity<UnionClipboardComponent> ent, ref UnionClipboardClaimLeadershipDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union!.Leader == args.Args.User)
            return;

        if (!_unionSelector.TrySetUnionLeader(union, args.Args.User))
            return;

        _popup.PopupEntity(Loc.GetString("union-clipboard-claimed-leadership", ("union", union.Name)), args.Args.User, args.Args.User);
        args.Handled = true;
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

        if (component.PendingStewardCandidate is { } pending)
        {
            VerifySteward((uid, component), union, pending, cardName, args.User);
            args.Handled = true;
            return;
        }

        // look at comment for this func to see why we do this in the first place
        var target = FindNearbyMatch(args.User, cardName);
        if (target == null)
        {
            _popup.PopupEntity(Loc.GetString("union-clipboard-no-match"), args.User, args.User);
            return;
        }

        if (idCard.Comp.JobDepartments.Any(dept => dept == CommandDepartment))
        {
            _popup.PopupEntity(Loc.GetString("union-clipboard-command-member", ("name", cardName)), args.User, args.User);
            return;
        }

        if (!_unionSelector.TryRegisterMember(union, target.Value))
        {
            _popup.PopupEntity(Loc.GetString("union-clipboard-already-registered"), args.User, args.User);
            return;
        }

        _unionSelector.GiveUnionCardToStorage(target.Value, union, uid);

        _popup.PopupEntity(Loc.GetString("union-clipboard-registered", ("name", cardName)), args.User, args.User);
        UpdateUi((uid, component));
        args.Handled = true;
    }

    private void VerifySteward(Entity<UnionClipboardComponent> ent, StationUnion union, EntityUid pending, string cardName, EntityUid leader)
    {
        if (!string.Equals(Name(pending), cardName, StringComparison.OrdinalIgnoreCase))
        {
            _popup.PopupEntity(Loc.GetString("union-clipboard-steward-mismatch"), leader, leader);
            return;
        }

        if (!union.Members.TryGetValue(pending, out var info))
        {
            ent.Comp.PendingStewardCandidate = null;
            UpdateUi(ent);
            return;
        }

        info.IsSteward = true;
        EnsureComp<UnionStewardComponent>(pending);
        _unionSelector.DirtyUnionVision();
        ent.Comp.PendingStewardCandidate = null;

        _popup.PopupEntity(Loc.GetString("union-clipboard-steward-confirmed", ("name", cardName)), leader, leader);
        UpdateUi(ent);
    }

    private void OnBeginSteward(Entity<UnionClipboardComponent> ent, ref UnionClipboardBeginStewardMessage args)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader != args.Actor)
            return;

        var target = GetEntity(args.Target);
        if (target == union.Leader || !union.Members.TryGetValue(target, out var info) || info.IsSteward)
            return;

        ent.Comp.PendingStewardCandidate = target;
        UpdateUi(ent);
    }

    private void OnCancelSteward(Entity<UnionClipboardComponent> ent, ref UnionClipboardCancelStewardMessage args)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader != args.Actor)
            return;

        ent.Comp.PendingStewardCandidate = null;
        UpdateUi(ent);
    }

    private void OnAssignSteward(Entity<UnionClipboardComponent> ent, ref UnionClipboardAssignStewardMessage args)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader != args.Actor)
            return;

        var target = GetEntity(args.Target);
        if (target == union.Leader || !union.Members.TryGetValue(target, out var info) || info.IsSteward)
            return;

        if (args.Steward is not { } stewardNet)
        {
            info.AssignedSteward = null;
            UpdateUi(ent);
            return;
        }

        var steward = GetEntity(stewardNet);
        if (steward != union.Leader && (!union.Members.TryGetValue(steward, out var stewardInfo) || !stewardInfo.IsSteward))
            return;

        info.AssignedSteward = steward;
        UpdateUi(ent);
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

            NetEntity? assignedSteward = info.AssignedSteward is { } stewardUid ? GetNetEntity(stewardUid) : null;
            members.Add(new UnionClipboardMemberEntry(GetNetEntity(memberUid), name, jobTitle!, memberUid == union.Leader, info.IsSteward, assignedSteward, notes));
        }

        string? lockedForName = null;
        if (ent.Comp.PendingStewardCandidate is { } pendingUid)
            lockedForName = Name(pendingUid);

        _ui.SetUiState(ent.Owner, UnionClipboardUiKey.Key, new UnionClipboardBoundUserInterfaceState(union.Name, members, lockedForName));
    }
}
