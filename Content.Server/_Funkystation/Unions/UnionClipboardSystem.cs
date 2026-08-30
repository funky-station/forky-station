using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared._Funkystation.Unions;
using Content.Shared.Access.Systems;
using Robust.Server.GameObjects;
using Robust.Shared.Timing;

namespace Content.Server._Funkystation.Unions;

public sealed partial class UnionClipboardSystem : EntitySystem
{
    [Dependency] private UnionSelectorSystem _unionSelector = default!;
    [Dependency] private SharedIdCardSystem _idCard = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private IGameTiming _timing = default!;

    private const int MaxNoteTitleLength = 64;
    private const int MaxNoteTextLength = 512;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UnionClipboardComponent, BoundUIOpenedEvent>(OnBoundUiOpened);
        Subs.BuiEvents<UnionClipboardComponent>(UnionClipboardUiKey.Key,
            subs =>
            {
                subs.Event<UnionClipboardRemoveMemberMessage>(OnRemoveMember);
                subs.Event<UnionClipboardAddNoteMessage>(OnAddNote);
            });
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
