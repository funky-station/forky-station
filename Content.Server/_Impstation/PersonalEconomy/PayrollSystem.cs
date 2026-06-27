using Content.Server.Chat.Systems;
using Content.Server.StationRecords.Systems;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared.CriminalRecords;
using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Shared.Timing;

namespace Content.Server._Impstation.PersonalEconomy;

/// <summary>
/// the payroll systemmmm
/// </summary>
public sealed class PayrollSystem : EntitySystem
{
    [Dependency] private readonly ServerBankingSystem _banking = null!;
    [Dependency] private readonly IGameTiming _timing = null!;
    [Dependency] private readonly SharedTransformSystem _xform = null!;
    [Dependency] private readonly ChatSystem _chat = null!;
    [Dependency] private readonly StationRecordsSystem _records = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StationPayrollComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(Entity<StationPayrollComponent> ent, ref ComponentStartup args)
    {
        if (ent.Comp.Initialized)
            return;

        // the station gets its own account to pay salaries out of
        var account = _banking.CreateNewAccount(Loc.GetString("nanobank-station-bank"), ent.Owner);
        account.Comp.IsStationAccount = true;
        Dirty(account);
        ent.Comp.StationAccount = account.Comp.AccountNumber;
        ent.Comp.NextPayout = _timing.CurTime + ent.Comp.PayoutInterval;
        ent.Comp.InitialFundTime = _timing.CurTime + ent.Comp.InitialFundDelay;
        ent.Comp.Initialized = true;
        Dirty(ent);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<StationPayrollComponent>();
        while (query.MoveNext(out var uid, out var payroll))
        {
            if (!payroll.Initialized)
                continue;

            if (!payroll.InitialFunded && _timing.CurTime >= payroll.InitialFundTime)
            {
                FundStationPool((uid, payroll));
                payroll.InitialFunded = true;
                Dirty(uid, payroll);
            }

            // only do the full station payout if its been long enough.
            if (_timing.CurTime < payroll.NextPayout)
                continue;

            RunCycle((uid, payroll));
            payroll.NextPayout = _timing.CurTime + payroll.PayoutInterval;
            Dirty(uid, payroll);
        }
    }

    private void FundStationPool(Entity<StationPayrollComponent> station)
    {
        if (!_banking.TryGetAccount(station.Comp.StationAccount, out var stationAccount))
            return;

        var eligible = 0;
        var query = EntityQueryEnumerator<BankAccountComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!BelongsToStation(uid, station.Owner) || comp.AccountNumber == station.Comp.StationAccount)
                continue;
            if (!ShouldWithhold(station.Owner, comp, out _))
                eligible++; // only pay the station for crew that is nice :)
        }

        var funding = station.Comp.BaseFunding + station.Comp.PerCrewFunding * eligible;
        _banking.SetAccountBalance(station.Comp.StationAccount, stationAccount.Value.Comp.Balance + funding);
    }

    private void RunCycle(Entity<StationPayrollComponent> station)
    {
        if (!_banking.TryGetAccount(station.Comp.StationAccount, out _))
            return;

        // funds the station for the cycle
        FundStationPool(station);

        // pay everyone!!!
        var query = EntityQueryEnumerator<BankAccountComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (!BelongsToStation(uid, station.Owner) || comp.AccountNumber == station.Comp.StationAccount)
                continue;
            if (comp.Salary <= 0)
                continue;

            if (ShouldWithhold(station.Owner, comp, out var reason))
            {
                _banking.LogWithheld(comp.AccountNumber, reason);
                continue;
            }

            _banking.TryMakeTransaction(station.Comp.StationAccount, comp.AccountNumber, comp.Salary, Loc.GetString("nanobank-salary-reason"));
        }

        //"Station" sender renders as a station announcement rather than a Central Command one
        _chat.DispatchStationAnnouncement(station.Owner, Loc.GetString("nanobank-payout-announcement"), Loc.GetString("nanobank-payout-sender"));
    }

    // pay is withheld for manually-suspended accounts and for wanted/detained crew
    private bool ShouldWithhold(EntityUid station, BankAccountComponent comp, out string reason)
    {
        if (comp.Status == PaymentStatus.Suspended)
        {
            reason = comp.StatusReason.Length != 0
                ? Loc.GetString("nanobank-withheld-reason", ("reason", comp.StatusReason))
                : Loc.GetString("nanobank-withheld");
            return true;
        }

        if (comp.StationRecordId != 0)
        {
            var key = new StationRecordKey(comp.StationRecordId, station);
            if (_records.TryGetRecord<CriminalRecord>(key, out var crim)
                && crim.Status is SecurityStatus.Wanted or SecurityStatus.Detained)
            {
                var status = Loc.GetString(crim.Status == SecurityStatus.Wanted
                    ? "nanobank-withheld-wanted"
                    : "nanobank-withheld-detained");
                reason = Loc.GetString("nanobank-withheld-reason", ("reason", status));
                return true;
            }
        }

        reason = string.Empty;
        return false;
    }

    private bool BelongsToStation(EntityUid account, EntityUid station)
    {
        return _xform.GetParentUid(account) == station;
    }
}
