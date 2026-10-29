using System.Linq;
using Content.Server.Station.Systems;
using Content.Shared._Funkystation.Pager;
using Content.Shared._Funkystation.Pager.Components;
using Content.Shared.Access.Systems;
using Content.Shared.CartridgeLoader;
using Content.Shared.Database;
using Content.Shared.StationRecords;
using Content.Shared.StationRecords.Events;

namespace Content.Server._Funkystation.Pager;

// pda app side of pagers
public sealed partial class PagerSystem
{
    [Dependency] private CartridgeLoaderSystem _cartridgeLoader = null!;
    [Dependency] private AccessReaderSystem _accessReader = null!;
    [Dependency] private StationSystem _station = null!;

    private void InitializeCartridge()
    {
        SubscribeLocalEvent<PagerCartridgeComponent, CartridgeMessageEvent>(OnCartridgeMessage);
        SubscribeLocalEvent<PagerCartridgeComponent, CartridgeUiReadyEvent>(OnCartridgeUiReady);
        SubscribeLocalEvent<RecordModifiedEvent>(OnRecordModified);
        SubscribeLocalEvent<RecordRemovedEvent>(OnRecordRemoved);
    }

    private void OnCartridgeUiReady(Entity<PagerCartridgeComponent> ent, ref CartridgeUiReadyEvent args)
    {
        UpdateCartridgeUi(ent, args.Loader);
    }

    private void OnCartridgeMessage(Entity<PagerCartridgeComponent> ent, ref CartridgeMessageEvent args)
    {
        if (!TryComp<PagerComponent>(ent, out var pagerComp))
            return;

        var pager = new Entity<PagerComponent>(ent, pagerComp);
        var loader = GetEntity(args.LoaderUid);

        switch (args)
        {
            case PagerCartridgeSendMessage send:
                TrySendPage(pager, args.Actor, send.TargetNumber, send.Code);
                break;
            case PagerCartridgeMassPageMessage mass:
                TryMassPage(pager, loader, args.Actor, mass.Prefix, mass.Code);
                break;
            case PagerCartridgeCycleModeMessage:
                CycleMode(pager, args.Actor);
                break;
        }

        UpdateCartridgeUi(ent, loader);
    }

    private void OnRecordModified(ref RecordModifiedEvent args)
    {
        RefreshCartridges();
    }

    private void OnRecordRemoved(ref RecordRemovedEvent args)
    {
        RefreshCartridges();
    }

    private void RefreshCartridges()
    {
        var query = EntityQueryEnumerator<PagerCartridgeComponent>();
        while (query.MoveNext(out var uid, out _))
        {
            RefreshCartridge(uid);
        }
    }

    private void RefreshCartridge(EntityUid uid)
    {
        if (!TryComp<CartridgeComponent>(uid, out var cartridge) || cartridge.LoaderUid is not { } loaderUid)
            return;

        if (!TryComp<CartridgeLoaderComponent>(loaderUid, out var loader) || loader.ActiveProgram != uid)
            return;

        UpdateCartridgeUi(uid, loaderUid);
    }

    private void UpdateCartridgeUi(EntityUid uid, EntityUid loaderUid)
    {
        if (!TryComp<PagerComponent>(uid, out var pagerComp))
            return;

        var pager = new Entity<PagerComponent>(uid, pagerComp);
        var state = new PagerCartridgeUiState(
            GetNumber(pager),
            GetMode(pager),
            GetCurrentPage(pager),
            BuildDirectories(uid, loaderUid));

        _cartridgeLoader.UpdateCartridgeUiState(loaderUid, state);
    }

    // one department the id can mass page, & its listed employees
    private List<PagerDirectory> BuildDirectories(EntityUid cartridge, EntityUid loader)
    {
        var result = new List<PagerDirectory>();
        var access = _accessReader.FindAccessTags(loader);
        var station = _station.GetOwningStation(cartridge);

        foreach (var proto in ProtoMan.EnumeratePrototypes<PagerPrefixPrototype>().OrderBy(p => p.Prefix))
        {
            if (!proto.MassPageAccess.Any(a => access.Contains(a)))
                continue;

            var entries = new List<PagerDirectoryEntry>();
            if (station is { } stationUid)
            {
                foreach (var (_, record) in _records.GetRecordsOfType<GeneralStationRecord>(stationUid))
                {
                    if (record.PagerNumber is not { } number || number / 100 != proto.Prefix)
                        continue;

                    entries.Add(new PagerDirectoryEntry(record.Name, record.JobTitle, number));
                }
            }

            entries.Sort((a, b) => a.Number.CompareTo(b.Number));
            result.Add(new PagerDirectory(ProtoMan.Index(proto.Department).Name, proto.Prefix, entries));
        }

        return result;
    }

    private bool CanMassPage(EntityUid loader, int prefix)
    {
        var access = _accessReader.FindAccessTags(loader);
        return ProtoMan.EnumeratePrototypes<PagerPrefixPrototype>()
            .Any(p => p.Prefix == prefix && p.MassPageAccess.Any(a => access.Contains(a)));
    }

    private void TryMassPage(Entity<PagerComponent> ent, EntityUid loader, EntityUid actor, int prefix, string? rawCode)
    {
        if (!CanMassPage(loader, prefix))
        {
            Popup.PopupEntity(Loc.GetString("pager-mass-no-access"), ent, actor);
            return;
        }

        if (!TryPrepareSend(ent, actor, rawCode, out var code, out var displaySenderNumber))
            return;

        _adminLogger.Add(LogType.Action, LogImpact.Medium, $"{ToPrettyString(actor):actor} mass paged prefix {prefix} from {ToPrettyString(ent):pager} (Real No. {GetNumber(ent)}, Displayed No. {displaySenderNumber}) with code '{code ?? "none"}'.");

        SendFeedback(ent, actor);

        var query = EntityQueryEnumerator<PagerComponent, TransformComponent>();
        while (query.MoveNext(out var recvUid, out var recvPager, out var recvXform))
        {
            if (recvUid == ent.Owner)
                continue;

            Entity<PagerComponent> receiver = (recvUid, recvPager);
            var number = GetNumber(receiver);

            if (number < 0 || number / 100 != prefix)
                continue;

            if (recvXform.GridUid is not { } recvGrid || !GridHasServer(recvGrid))
                continue;

            DeliverPage(receiver, displaySenderNumber, code);
        }
    }

    // pda apps ping through the pda
    private bool TryNotifyCartridge(EntityUid receiver, int senderNumber, string? code)
    {
        if (!HasComp<PagerCartridgeComponent>(receiver) ||
            !TryComp<CartridgeComponent>(receiver, out var cartridge) ||
            cartridge.LoaderUid is not { } loaderUid ||
            !TryComp<CartridgeLoaderComponent>(loaderUid, out var loader))
            return false;

        if (!loader.NotificationsEnabled)
        {
            _ringer.RingerPlayRingtone(loaderUid);
            return true;
        }

        var line = Loc.GetString(
            code != null ? "pager-window-log-line-coded" : "pager-window-log-line",
            ("sender", FormatNumber(senderNumber)),
            ("code", code ?? string.Empty));

        _cartridgeLoader.SendNotification(loaderUid, Loc.GetString("pager-cartridge-notification-header"), line, loader);
        return true;
    }
}
