using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared._Funkystation.Traits.Unions;
using Content.Shared._Funkystation.Unions;
using Content.Shared.CCVar;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;
using Content.Shared.Timing;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;

namespace Content.Server._Funkystation.Unions;

public sealed partial class MegaphoneSystem : EntitySystem
{
    [Dependency] private UnionSelectorSystem _unionSelector = default!;
    [Dependency] private UseDelaySystem _useDelay = default!;
    [Dependency] private ChatSystem _chatSystem = default!;
    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MegaphoneComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<MegaphoneComponent, MegaphoneClaimLeadershipDoAfterEvent>(OnClaimLeadershipDoAfter);
        SubscribeLocalEvent<MegaphoneComponent, BoundUIOpenedEvent>(OnBoundUiOpened);

        Subs.BuiEvents<MegaphoneComponent>(MegaphoneUiKey.Key,
            subs => subs.Event<MegaphoneCallStrikeMessage>(OnCallStrike));
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
            _ui.TryToggleUi(uid, MegaphoneUiKey.Key, args.User);
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

    private void OnBoundUiOpened(Entity<MegaphoneComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnCallStrike(Entity<MegaphoneComponent> ent, ref MegaphoneCallStrikeMessage args)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        if (union.Leader != args.Actor)
            return;

        if (TryComp<UseDelayComponent>(ent.Owner, out var useDelay) && !_useDelay.TryResetDelay((ent.Owner, useDelay), true))
        {
            _popup.PopupEntity(Loc.GetString("megaphone-on-cooldown"), args.Actor, args.Actor);
            return;
        }

        union.OnStrike = !union.OnStrike;

        string message;
        if (union.OnStrike)
        {
            var maxLength = _cfg.GetCVar(CCVars.ChatMaxAnnouncementLength);
            var text = args.Text.Trim();
            if (text.Length > maxLength)
                text = text[..maxLength];

            message = text.Length > 0
                ? text
                : Loc.GetString("megaphone-strike-started", ("union", union.Name));
        }
        else
        {
            message = Loc.GetString("megaphone-strike-ended", ("union", union.Name));
        }
        
        // so it look pretty just like the comms console one
        var author = $"{Name(args.Actor)} ({_unionSelector.GetUnionDisplayName(union)})";
        message += "\n" + Loc.GetString("comms-console-announcement-sent-by") + " " + author;

        _chatSystem.DispatchStationAnnouncement(ent.Owner, message, sender: union.Name);
        UpdateUi(ent);
    }

    private void UpdateUi(Entity<MegaphoneComponent> ent)
    {
        if (!_unionSelector.TryGetUnionForGrouping(ent.Comp.GroupingId, out var union) || union == null)
            return;

        _ui.SetUiState(ent.Owner, MegaphoneUiKey.Key, new MegaphoneBoundUserInterfaceState(union.OnStrike));
    }
}
