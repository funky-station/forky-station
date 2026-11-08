using System.Numerics;
using Content.Client._Impstation.PersonalEconomy.UI;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Station;
using Robust.Client.UserInterface;

namespace Content.Client._Impstation.PersonalEconomy.BUI;

public sealed class ATMBoundUserInterface : BoundUserInterface
{

    private readonly ClientBankingSystem _banking;
    private readonly ItemSlotsSystem _itemSlots;
    private readonly SharedStationSystem _station;

    private ATMMenu? _atmMenu;
    private TransactionMenu? _transactionMenu;
    private PinEntryMenu? _pinMenu;
    private Entity<BankAccountComponent>? _account;
    private int _pendingWithdrawAmount;

    public ATMBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _banking = EntMan.System<ClientBankingSystem>();
        _itemSlots = EntMan.System<ItemSlotsSystem>();
        _station = EntMan.System<SharedStationSystem>();
    }

    protected override void Open()
    {
        base.Open();

        _atmMenu = this.CreateWindow<ATMMenu>();
        _atmMenu.OnClose += ClearMenu;

        _atmMenu.OnInsertCardPressed += () => SendPredictedMessage(new InsertCardMessage());
        _atmMenu.OnEjectCardPressed += () => SendPredictedMessage(new EjectCardMessage());
        _atmMenu.OnDepositPressed += () => SendPredictedMessage(new DepositMessage());

        //withdrawing pops a keypad window for the PIN, then sends the request
        _pinMenu = new PinEntryMenu();
        _pinMenu.OnPinEntered += pin => SendPredictedMessage(new WithdrawMessage(_pendingWithdrawAmount, pin));
        _atmMenu.OnWithdrawPressed += amount =>
        {
            _pendingWithdrawAmount = amount;
            _pinMenu.Reset();
            _pinMenu.Open();
        };

        _transactionMenu = new TransactionMenu();

        _atmMenu.OnTransactionButtonPressed += () =>
        {
            if (_transactionMenu.IsOpen)
            {
                _transactionMenu.Close();
                return;
            }

            _transactionMenu.TransferNumberBox.Clear();
            _transactionMenu.TransferAmountBox.Clear();
            _transactionMenu.TransferReasonBox.Clear();
            _transactionMenu.TransferAmount = 0;
            _transactionMenu.ReallyConfirmButton.Disabled = true;
            _transactionMenu.TransactionNotEnoughFundsLabel.Visible = false;
            _transactionMenu.TransactionRecipientDoesNotExistLabel.Visible = false;

            //todo make this part of the window and not a separate thing that just gets plonked next to it
            //like the news manager console but I have no fucking idea how that does that tbh
            _transactionMenu.Open(_atmMenu.Position + new Vector2(_atmMenu.Width, 0));
        };

        _transactionMenu.TransactionConfirmAttempt += () =>
        {
            //todo make this a method yada yada
            if (!VerifyTransaction(_transactionMenu.TransferNumberBox.Text, _transactionMenu.TransferAmount))
            {
                _transactionMenu.ReallyConfirmButton.Disabled = true;
                return;
            }

            _transactionMenu.ReallyConfirmButton.Disabled = false;
        };

        _transactionMenu.TransactionConfirmed += () =>
        {
            //re-verify for safety
            if (!VerifyTransaction(_transactionMenu.TransferNumberBox.Text, _transactionMenu.TransferAmount))
            {
                _transactionMenu.ReallyConfirmButton.Disabled = true;
                return;
            }

            _transactionMenu.ReallyConfirmButton.Disabled = false;

            // sender comes from the inserted card on the server
            SendPredictedMessage(
                new RequestTransactionMessage(
                    int.Parse(_transactionMenu.TransferNumberBox.Text),
                    _transactionMenu.TransferAmount,
                    _transactionMenu.TransferReasonBox.Text)
                );

            //finally, close the menu after a theoretically successful transaction
            _transactionMenu.Close();
        };

        Update();
    }

    public override void Update()
    {
        if (_atmMenu == null || !EntMan.TryGetComponent<ATMComponent>(Owner, out var atm))
            return;

        var cardUid = _itemSlots.GetItemOrNull(Owner, atm.CardSlotId);
        _account = null;
        if (cardUid != null
            && EntMan.TryGetComponent<BankCardComponent>(cardUid, out var card)
            && _banking.TryGetAccount(card.AccountNumber, out var account))
        {
            _account = account.Value;
        }

        _atmMenu.UpdateState(_account, cardUid != null, GetPayroll());
    }

    private StationPayrollComponent? GetPayroll()
    {
        var station = _station.GetOwningStation(Owner);
        return EntMan.TryGetComponent<StationPayrollComponent>(station, out var payroll) ? payroll : null;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        _atmMenu?.Dispose();
        _transactionMenu?.Dispose();
        _pinMenu?.Dispose();
    }

    private void ClearMenu()
    {
        _account = null;
        _transactionMenu?.Close();
    }

    //todo move this into banking system
    //actually this kinda can't get moved into there since it needs to convey UI-specific info? maybe have an overload that returns a TransactionFailureReason enum or smth?
    private bool VerifyTransaction(string recipient, int amount)
    {
        var verified = true;
        // recipient is addressed by their 6-digit account number
        if (recipient.Length != 6 ||
            !int.TryParse(recipient, out var number) ||
            !_banking.TryGetAccount(number, out _) //does the recipient exist?
           )
        {
            _transactionMenu!.TransactionRecipientDoesNotExistLabel.Visible = true;
            verified = false;
        }

        // do we have enough money to make the transfer?
        if (_account == null || _account.Value.Comp.Balance < amount)
        {
            _transactionMenu!.TransactionNotEnoughFundsLabel.Visible = true;
            verified = false;
        }

        return verified;
    }
}
