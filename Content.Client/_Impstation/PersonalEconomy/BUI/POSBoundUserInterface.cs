using Content.Client._Impstation.PersonalEconomy.UI.POS;
using Content.Shared.CCVar;
using Content.Shared._Impstation.PersonalEconomy.Components;
using Content.Shared._Impstation.PersonalEconomy.Events;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;

namespace Content.Client._Impstation.PersonalEconomy.BUI;

public sealed class POSBoundUserInterface : BoundUserInterface
{
    private readonly ClientBankingSystem _banking;
    private readonly IConfigurationManager _cfg;
    private PoSMenu? _menu;
    private TipMenu? _tipMenu;

    public POSBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
        _banking = EntMan.System<ClientBankingSystem>();
        _cfg = IoCManager.Resolve<IConfigurationManager>();
    }

    protected override void Open()
    {
        base.Open();

        var comp = EntMan.GetComponent<PosSystemComponent>(Owner);

        _menu = this.CreateWindow<PoSMenu>();

        // owner of the pos opens up the locked pin view
        if (IsOwner(comp) || comp.RecipientAccount == 0)
            ShowLockBox();
        else
            ShowCustomerBox();

        _menu.OnClearButtonPressed += () =>
        {
            if (EntMan.GetComponent<PosSystemComponent>(Owner).RecipientAccount != 0)
                ShowCustomerBox();
        };
    }

    protected override void ReceiveMessage(BoundUserInterfaceMessage message)
    {
        if (message is PosUnlockedMessage)
            ShowSetupBox();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _tipMenu?.Close();
    }

    private bool IsOwner(PosSystemComponent comp)
    {
        // owner must be holding their card to access the settings view
        return comp.OwnerAccount != 0 && _banking.LocalHoldsAccount(comp.OwnerAccount.Number);
    }

    // this is what the owner of the keypad sees
    private void ShowLockBox()
    {
        var claimed = EntMan.GetComponent<PosSystemComponent>(Owner).OwnerAccount != 0;
        _menu!.CreateLockBox(claimed);
        _menu.OnNumberEntered = s =>
        {
            if (int.TryParse(s, out var pin))
                SendPredictedMessage(new UnlockPosMessage(pin));
            //wipe the entry so a wrong PIN doesn't linger for the next attempt
            _menu.ClearKeypad();
        };
    }

    // this what customers see
    private void ShowCustomerBox()
    {
        var box = _menu!.CreateInvalidPaymentBox();
        box.MerchantPressed += ShowLockBox;
        _menu.OnNumberEntered = OnCustomerAccountEntered;
    }

    private void OnCustomerAccountEntered(string s)
    {
        var comp = EntMan.GetComponent<PosSystemComponent>(Owner);
        if (comp.RecipientAccount == 0)
            return;

        if (!int.TryParse(s, out var customerAccount) || !_banking.TryGetAccount(customerAccount, out var account))
        {
            ShowCustomerBox();
            return;
        }

        // the recipient cant pay themselves
        if (comp.RecipientAccount == account.Value.Comp.AccountNumber)
            return;

        ShowPaymentBox(comp, customerAccount);
    }

    private void ShowPaymentBox(PosSystemComponent comp, int customerAccount)
    {
        var box = _menu!.CreatePaymentBox();
        //just assume that the bank account will not be null at this point. god help me for when I get around to deleting accounts (:
        //todo make this whole UI account for the fact that these accounts could all get deleted at some point
        _banking.TryGetAccount(comp.RecipientAccount, out var recipient);
        var merchantName = string.IsNullOrWhiteSpace(comp.MerchantName) ? recipient!.Value.Comp.Name : comp.MerchantName;
        var tax = _banking.PosTaxFor(comp.Amount);
        box.FillOutDetails(merchantName, recipient!.Value.Comp.AccountNumber, comp.Amount, tax, _cfg.GetCVar(CCVars.PosTax), comp.Reason);
        box.TransactionCancelled += Close;

        box.TransactionConfirmed += () =>
        {
            // the customer needs to cover the subtotal plus tax
            if (!VerifyTransaction(comp.RecipientAccount, customerAccount, comp.Amount + tax))
            {
                box.NoFundsLabel.Visible = true;
                SendPredictedMessage(new PoSTransactionFailedMessage());
                return;
            }

            box.NoFundsLabel.Visible = false;
            SendPredictedMessage(new PoSTransactionSuccededMessage());
            //the sales done, let them tip!!
            _menu!.Visible = false;
            OpenTipMenu(comp.Amount);
        };
    }

    private void ShowSetupBox()
    {
        var comp = EntMan.GetComponent<PosSystemComponent>(Owner);
        var box = _menu!.CreateSetupBox();
        _menu.OnNumberEntered = null;

        //prefill from the comp if it's already configured, otherwise seed the recipient with the owner's account
        if (comp.RecipientAccount != 0)
            box.FillOutDetails(comp.RecipientAccount, comp.Amount, comp.Reason, comp.MerchantName);
        else if (comp.OwnerAccount != 0)
            box.TransferNoEntryBox.Text = $"{comp.OwnerAccount.Number:000000}";

        box.OnSetupCleared += () =>
        {
            SendPredictedMessage(new UpdatePoSSettingsMessage(0, 0, "", ""));
            ShowLockBox();
        };

        box.OnSetupConfirmed += () =>
        {
            var valid = true;
            //if the recipient doesn't exist, say what's going wrong and mark this as invalid
            if (!VerifyRecipient(box.TransferNoEntryBox.Text, out var number))
            {
                box.InvalidRecipientLabel.Visible = true;
                valid = false;
            }

            //if the transfer amount is 0, say what's going on and mark this as invalid
            if (box.TransferAmount == 0)
            {
                box.InvalidTransferAmountLabel.Visible = true;
                valid = false;
            }

            if (!valid)
            {
                box.SetupConfirmedLabel.Visible = false;
                return;
            }

            box.InvalidRecipientLabel.Visible = false;
            box.InvalidTransferAmountLabel.Visible = false;
            box.SetupConfirmedLabel.Visible = true;

            SendPredictedMessage(new UpdatePoSSettingsMessage(number, box.TransferAmount, box.TransferReasonEntryBox.Text, box.MerchantNameEntryBox.Text));
        };
    }

    // shows tip on purchase lol
    private void OpenTipMenu(int subtotal)
    {
        _tipMenu?.Close();
        _tipMenu = new TipMenu();
        _tipMenu.SetSubtotal(subtotal);
        _tipMenu.OnTipChosen += amount =>
        {
            if (amount > 0)
                SendPredictedMessage(new PoSTipMessage(amount));
            Close();
        };
        _tipMenu.OpenCentered();
    }

    //todo these should probably be in a helpers file?
    private bool VerifyRecipient(string recipient, out int recipientNumber)
    {
        recipientNumber = 0;

        if (recipient.Length != 6 || !int.TryParse(recipient, out var number) || !_banking.TryGetAccount(number, out _))
            return false;

        recipientNumber = number;
        return true;
    }

    private bool VerifyTransaction(int recipient, int sender, int amount)
    {
        return _banking.TryGetAccount(recipient, out _)
            && _banking.TryGetAccount(sender, out var senderAcc)
            && senderAcc.Value.Comp.Balance >= amount;
    }
}
