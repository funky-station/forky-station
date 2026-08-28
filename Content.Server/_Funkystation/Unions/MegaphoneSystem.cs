using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;
using Content.Shared.Timing;

namespace Content.Server._Funkystation.Unions;

public sealed partial class MegaphoneSystem : EntitySystem
{
    [Dependency] private UnionSelectorSystem _unionSelector = default!;
    [Dependency] private UseDelaySystem _useDelay = default!;
    [Dependency] private ChatSystem _chatSystem = default!;
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MegaphoneComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<MegaphoneComponent, MegaphoneClaimLeadershipDoAfterEvent>(OnClaimLeadershipDoAfter);
    }

    private void OnUseInHand(EntityUid uid, MegaphoneComponent component, UseInHandEvent args)
    {
        if (args.Handled)
            return;
        args.Handled = true;

        if (!_unionSelector.TryGetUnionForGrouping(component.GroupingId, out var union))
            return;

        if (union!.Leader == args.User)
        {
            ToggleStrike(uid, component, union, args.User);
            return;
        }

        if (!union.Members.ContainsKey(args.User))
        {
            _popup.PopupEntity(Loc.GetString("megaphone-not-a-member"), args.User, args.User);
            return;
        }

        _popup.PopupEntity(Loc.GetString("megaphone-claim-leadership-prompt"), args.User, args.User);

        var doAfterArgs = new DoAfterArgs(EntityManager, args.User, component.ClaimLeadershipDelay, new MegaphoneClaimLeadershipDoAfterEvent(), uid, used: uid)
        {
            BreakOnMove = true,
            NeedHand = true,
        };
        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnClaimLeadershipDoAfter(EntityUid uid, MegaphoneComponent component, MegaphoneClaimLeadershipDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        if (!_unionSelector.TryGetUnionForGrouping(component.GroupingId, out var union) || union!.Leader == args.Args.User)
            return;

        if (!_unionSelector.TrySetUnionLeader(union, args.Args.User))
            return;

        _popup.PopupEntity(Loc.GetString("megaphone-claimed-leadership", ("union", union.Name)), args.Args.User, args.Args.User);
        args.Handled = true;
    }

    private void ToggleStrike(EntityUid uid, MegaphoneComponent component, StationUnion union, EntityUid user)
    {
        if (TryComp<UseDelayComponent>(uid, out var useDelay) && !_useDelay.TryResetDelay((uid, useDelay), true))
        {
            _popup.PopupEntity(Loc.GetString("megaphone-on-cooldown"), user, user);
            return;
        }

        union.OnStrike = !union.OnStrike;

        var message = union.OnStrike
            ? Loc.GetString("megaphone-strike-started", ("union", union.Name))
            : Loc.GetString("megaphone-strike-ended", ("union", union.Name));
        
        // todo: change how this works since we want to avoid messages like this
        // or is it ok like this?
        _chatSystem.DispatchStationAnnouncement(uid, message, sender: union.Name);
    }
}
