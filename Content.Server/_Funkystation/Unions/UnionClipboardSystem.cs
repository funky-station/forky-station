using Content.Server.Popups;
using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared._Funkystation.Unions;
using Content.Shared.Access.Systems;
using Robust.Server.GameObjects;

namespace Content.Server._Funkystation.Unions;

public sealed partial class UnionClipboardSystem : EntitySystem
{
    [Dependency] private UnionSelectorSystem _unionSelector = default!;
    [Dependency] private SharedIdCardSystem _idCard = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<UnionClipboardComponent, BoundUIOpenedEvent>(OnBoundUiOpened);
        Subs.BuiEvents<UnionClipboardComponent>(UnionClipboardUiKey.Key,
            subs =>
            {
                subs.Event<UnionClipboardRemoveMemberMessage>(OnRemoveMember);
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

    private void UpdateUi(Entity<UnionClipboardComponent> ent)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        var members = new List<UnionClipboardMemberEntry>();
        foreach (var memberUid in union.Members.Keys)
        {
            var name = Name(memberUid);
            var jobTitle = string.Empty;

            if (_idCard.TryFindIdCard(memberUid, out var idCard))
            {
                name = idCard.Comp.FullName ?? name;
                jobTitle = idCard.Comp.LocalizedJobTitle;
            }

            members.Add(new UnionClipboardMemberEntry(GetNetEntity(memberUid), name, jobTitle!, memberUid == union.Leader));
        }

        _ui.SetUiState(ent.Owner, UnionClipboardUiKey.Key, new UnionClipboardBoundUserInterfaceState(union.Name, members));
    }
}
