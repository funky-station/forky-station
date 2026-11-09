using Content.Client._Impstation.PersonalEconomy.UI.AccountManagement;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Station;
using Robust.Client.UserInterface;

namespace Content.Client._Impstation.PersonalEconomy.BUI;

public sealed class AccountManagementConsoleBoundUserInterface : BoundUserInterface
{
    private readonly ItemSlotsSystem _itemSlots;
    private readonly SharedStationSystem _station;

    private AccountManagementMenu? _window;

    public AccountManagementConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _itemSlots = EntMan.System<ItemSlotsSystem>();
        _station = EntMan.System<SharedStationSystem>();
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<AccountManagementMenu>();
        _window.OpenCentered();
        _window.Populate();

        _window.GetInsertedCard = GetInsertedCard;
        _window.GetPayroll = GetPayroll;

        _window.OnSetStatus += (account, status, reason) =>
            SendPredictedMessage(new SetAccountStatusMessage(account, status, reason));
        _window.OnSetSalary += (account, salary) =>
            SendPredictedMessage(new SetAccountSalaryMessage(account, salary));
        _window.OnGrantBonus += (account, amount) =>
            SendPredictedMessage(new GrantAccountBonusMessage(account, amount));
        _window.OnInsertCard += () => SendPredictedMessage(new InsertCardMessage());
        _window.OnEjectCard += () => SendPredictedMessage(new EjectCardMessage());
        _window.OnWriteCard += account => SendPredictedMessage(new WriteCardMessage(account));
        _window.OnCreateAccount += name => SendMessage(new CreateBusinessAccountMessage(name));
        _window.OnSetDepartmentStatus += (dept, status, reason) =>
            SendPredictedMessage(new SetDepartmentStatusMessage(dept, status, reason));
        _window.OnGrantDepartmentBonus += (dept, amount) =>
            SendPredictedMessage(new GrantDepartmentBonusMessage(dept, amount));
        _window.OnCashOut += amount => SendPredictedMessage(new ConvertStationScripMessage(amount));
    }

    private StationPayrollComponent? GetPayroll()
    {
        var station = _station.GetOwningStation(Owner);
        if (station == null || !EntMan.TryGetComponent<StationPayrollComponent>(station.Value, out var payroll))
            return null;

        return payroll;
    }

    private BankCardComponent? GetInsertedCard()
    {
        if (!EntMan.TryGetComponent<AccountManagementConsoleComponent>(Owner, out var console))
            return null;

        var cardUid = _itemSlots.GetItemOrNull(Owner, console.CardSlotId);
        if (cardUid == null || !EntMan.TryGetComponent<BankCardComponent>(cardUid, out var card))
            return null;

        return card;
    }
}
