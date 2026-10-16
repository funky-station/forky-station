using System.Linq;
using Content.Server._Funkystation.StationRecords.Components;
using Content.Server.Power.Components;
using Content.Shared._Funkystation.Pager;
using Content.Shared._Funkystation.Pager.Components;
using Content.Shared.PDA;
using Content.Shared.PDA.Ringer;
using Content.Shared.StationRecords;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Server.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Emag.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.DeviceLinking.Systems;
using Content.Shared.StationRecords.Events;
using Content.Shared.StationRecords.Systems;
using Content.Shared.Stunnable;
using Content.Shared.Roles.Jobs;

namespace Content.Server._Funkystation.Pager;

public sealed partial class PagerSystem : SharedPagerSystem
{
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] private IRobustRandom _random = null!;
    [Dependency] private UserInterfaceSystem _ui = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedRingerSystem _ringer = null!;
    [Dependency] private StationRecordsSystem _records = null!;
    [Dependency] private IAdminLogManager _adminLogger = null!;
    [Dependency] private ExplosionSystem _explosion = null!;
    [Dependency] private DeviceLinkSystem _deviceLink = null!;
    [Dependency] private SharedStunSystem _stun = null!;
    [Dependency] private SharedJobSystem _jobs = null!;

    private readonly HashSet<int> _assignedNumbers = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PagerComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<PagerComponent, PagerSendPageMessage>(OnSendPage);
        SubscribeLocalEvent<PagerComponent, BoundUIOpenedEvent>(OnBuiOpened);
        SubscribeLocalEvent<GeneralRecordCreatedEvent>(OnGeneralRecordCreated);
        SubscribeLocalEvent<PagerComponent, GotEmaggedEvent>(OnEmagged);

        InitializeCartridge();
    }

    private void OnEmagged(Entity<PagerComponent> ent, ref GotEmaggedEvent args)
    {
        if (ent.Comp.Emagged)
            return;

        ent.Comp.Emagged = true;
        Dirty(ent);
        args.Handled = true;
    }

    private void OnGeneralRecordCreated(ref GeneralRecordCreatedEvent args)
    {
        if (!_records.TryGetRecord<GeneralStationRecord>(args.Key, out var record))
            return;

        if (FindPagerForRecord(record) is not { } pager)
            return;

        var number = GetNumber(pager);

        // department prefix, no prefix keeps the random number
        if (TryGetJobPrefix(record.JobPrototype, out var prefix) && number / 100 != prefix)
            number = AssignNumber(pager, prefix);

        record.PagerNumber = number;

        if (TryComp<XoRecordManifestComponent>(args.Key.OriginStation, out var manifest) &&
            manifest.Published.TryGetValue(args.Key.Id, out var published))
        {
            manifest.Published[args.Key.Id] = published with { PagerNumber = number };
        }
        RefreshCartridges();
    }

    private Entity<PagerComponent>? FindPagerForRecord(GeneralStationRecord record)
    {
        var query = EntityQueryEnumerator<PagerComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var pager, out var xform))
        {
            var parent = xform.ParentUid;
            while (parent.IsValid())
            {
                if (Name(parent) == record.Name && pager.Number != -1)
                    return new Entity<PagerComponent>(uid, pager);

                parent = Transform(parent).ParentUid;
            }
        }
        return null;
    }

    private Dictionary<string, int> GetPrefixes()
    {
        var prefixes = new Dictionary<string, int>();
        foreach (var proto in ProtoMan.EnumeratePrototypes<PagerPrefixPrototype>())
        {
            prefixes[proto.Department.Id] = proto.Prefix;
        }
        return prefixes;
    }

    // heads count as command first, not including commandant
    private bool TryGetJobPrefix(string jobId, out int prefix)
    {
        prefix = 0;
        if (string.IsNullOrEmpty(jobId) || !_jobs.TryGetSecondaryDepartmentsOrFallback(jobId, out var departments))
            return false;

        var prefixes = GetPrefixes();
        foreach (var department in departments)
        {
            if (prefixes.TryGetValue(department.ID, out prefix))
                return true;
        }

        prefix = 0;
        return false;
    }

    private void OnBuiOpened(Entity<PagerComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (args.UiKey is PagerUiKey.Key)
            PushUiState(ent);
    }

    private void PushUiState(Entity<PagerComponent> ent)
    {
        if (HasComp<PagerCartridgeComponent>(ent))
        {
            RefreshCartridge(ent.Owner);
            return;
        }

        var state = new PagerBoundUserInterfaceState(GetNumber(ent), GetMode(ent), GetCurrentPage(ent));
        _ui.SetUiState(ent.Owner, PagerUiKey.Key, state);
    }

    private void OnMapInit(Entity<PagerComponent> ent, ref MapInitEvent args)
    {
        _deviceLink.EnsureSourcePorts(ent.Owner, "PagerSender");

        var existing = GetNumber(ent);
        if (existing != -1)
        {
            _assignedNumbers.Add(existing);
            return;
        }

        SetNumber(ent, GetUnusedNumber());
    }

    private int AssignNumber(Entity<PagerComponent> ent, int? prefix = null)
    {
        var existing = GetNumber(ent);
        if (existing != -1)
            _assignedNumbers.Remove(existing);

        var number = GetUnusedNumber(prefix);
        SetNumber(ent, number);
        return number;
    }

    private int GetUnusedNumber(int? prefix = null)
    {
        if (prefix is { } pre)
        {
            var start = pre * 100;
            for (var i = 0; i < 100; i++)
            {
                var candidate = start + (_random.Next(0, 100) + i) % 100;
                if (!_assignedNumbers.Add(candidate))
                    continue;

                return candidate;
            }
        }

        var reserved = GetPrefixes().Values.ToHashSet();
        var number = _random.Next(MinNumber, MaxNumber + 1);
        var attempts = 0;
        while ((reserved.Contains(number / 100) || _assignedNumbers.Contains(number)) && attempts < 2000)
        {
            number = _random.Next(MinNumber, MaxNumber + 1);
            attempts++;
        }

        _assignedNumbers.Add(number);
        return number;
    }

    private void OnSendPage(Entity<PagerComponent> ent, ref PagerSendPageMessage args)
    {
        TrySendPage(ent, args.Actor, args.TargetNumber, args.Code);
    }

    private void TrySendPage(Entity<PagerComponent> ent, EntityUid actor, int targetNumber, string? rawCode)
    {
        if (!IsValidNumber(targetNumber))
            return;

        if (!TryPrepareSend(ent, actor, rawCode, out var code, out var displaySenderNumber))
            return;

        _adminLogger.Add(LogType.Action, LogImpact.Low, $"{ToPrettyString(actor):actor} sent page from {ToPrettyString(ent):pager} (Real No. {GetNumber(ent)}, Displayed No. {displaySenderNumber}) to #{targetNumber} with code '{code ?? "none"}'.");

        SendFeedback(ent, actor);

        var query = EntityQueryEnumerator<PagerComponent, TransformComponent>();
        while (query.MoveNext(out var recvUid, out var recvPager, out var recvXform))
        {
            Entity<PagerComponent> receiver = (recvUid, recvPager);

            if (GetNumber(receiver) != targetNumber)
                continue;

            if (recvXform.GridUid is not { } recvGrid || !GridHasServer(recvGrid))
                continue;

            DeliverPage(receiver, displaySenderNumber, code);
        }
    }

    // checks shared by single and mass pages, false means nothing goes out
    private bool TryPrepareSend(Entity<PagerComponent> ent, EntityUid actor, string? rawCode, out string? code, out int displaySenderNumber)
    {
        code = null;
        displaySenderNumber = -1;

        if (!IsValidCode(rawCode))
            return false;

        if (!TryConsumeCooldown(ent, _timing.CurTime))
        {
            Popup.PopupEntity(Loc.GetString("pager-send-too-fast"), ent, actor);
            return false;
        }

        var senderXform = Transform(ent);
        if (senderXform.GridUid is not { } senderGrid || !GridHasServer(senderGrid))
        {
            Popup.PopupEntity(Loc.GetString("pager-no-signal"), ent, actor);
            return false;
        }

        displaySenderNumber = ent.Comp.Emagged ? _random.Next(MinNumber, MaxNumber + 1) : GetNumber(ent);

        var cleanCode = rawCode?.Trim().ToUpperInvariant();
        if (string.IsNullOrEmpty(cleanCode))
            cleanCode = null;

        if (cleanCode != null && ent.Comp.Blacklist.Exists(x => x.Equals(cleanCode, StringComparison.InvariantCultureIgnoreCase)))
        {
            _adminLogger.Add(LogType.Action, LogImpact.High, $"{ToPrettyString(actor):actor} triggered blacklist explosion on {ToPrettyString(ent):pager} with code '{cleanCode}'.");

            _stun.TryKnockdown(actor, TimeSpan.FromSeconds(10));

            _explosion.QueueExplosion(
                ent.Owner,
                "Default",
                totalIntensity: 5,
                slope: 5,
                maxTileIntensity: 5,
                tileBreakScale: 0f,
                maxTileBreak: 0,
                canCreateVacuum: false,
                user: actor
            );

            QueueDel(ent);
            return false;
        }

        code = cleanCode;
        return true;
    }

    // sound n popup
    private void SendFeedback(Entity<PagerComponent> ent, EntityUid user)
    {
        _audio.PlayPvs(GetSendSound(ent), ent);
        Popup.PopupEntity(Loc.GetString("pager-page-sent"), ent, user);
    }

    private void DeliverPage(Entity<PagerComponent> receiver, int senderNumber, string? code)
    {
        SetCurrentPage(receiver, senderNumber, code, _timing.CurTime);
        PushUiState(receiver);

        switch (GetMode(receiver))
        {
            case PagerMode.Beep:
                // pda app rings through the pda
                if (TryNotifyCartridge(receiver, senderNumber, code))
                    break;

                if (TryComp<RingerComponent>(receiver, out var ringer))
                {
                    _ringer.RingerPlayRingtone((receiver, ringer));
                }
                else
                {
                    _audio.PlayPvs(GetBeepSound(receiver), receiver);
                    Popup.PopupEntity(Loc.GetString("pager-page-received"), receiver);
                }
                break;
            case PagerMode.Buzz:
                _audio.PlayPvs(GetBuzzSound(receiver), receiver);
                break;
            case PagerMode.Mute:
                break;
        }

        _deviceLink.InvokePort(receiver.Owner, "PagerSender");
    }

    private bool GridHasServer(EntityUid grid)
    {
        var query = EntityQueryEnumerator<PagerServerComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out _, out var xform))
        {
            if (xform.GridUid != grid)
                continue;

            if (!TryComp<ApcPowerReceiverComponent>(uid, out var power) || !power.Powered)
                continue;

            return true;
        }

        return false;
    }
}
