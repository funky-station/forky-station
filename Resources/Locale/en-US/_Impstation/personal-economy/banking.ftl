#bank card stuff
bank-card-name = {$name}'s bank card
bank-card-description = This bank card belongs to {$name}.
bank-card-slot-name = Bank Card
bank-card-examine-account-number = The account number is {$number}
bank-card-examine-balance = The account's balance is {$balance} scrip
bank-card-examine-salary = The account's salary is {$salary} scrip
bank-pin-notify = Your bank account is #{$account}. Your PIN is {$pin} - keep it secret!

# character info menu
bank-character-heading = Bank Account
bank-character-account = Account Number: #{$number}
bank-character-pin = PIN: {$pin}

#atm stuff
atm-machine-window-title = Automated Teller Machine
atm-machine-no-transactions = This Account Has Made No Transactions
atm-invalid-account-number = Please insert a bank card to continue
atm-card-unprogrammed = This card is not programmed, take it to the Head of Personnel!
atm-insert-card-button = Insert Card
atm-eject-card-button = Eject Card
atm-deposit-button = Deposit
atm-withdraw-button = Withdraw
atm-withdraw-amount-placeholder = Amount
atm-pin-entry-title = Enter PIN
atm-pin-entry-prompt = Enter your 4-digit PIN to withdraw.

#NanoBank branded stuff
nanobank-title = NanoBank
nanobank-tagline = The only bank you'll need.
nanobank-welcome = Welcome, {$name}!
nanobank-balance-label = Balance
nanobank-balance-amount = N$ {$balance}
nanobank-recent-transactions = Recent Transactions
nanobank-transaction-amount-in = +N$ {$amount}
nanobank-transaction-amount-out = -N$ {$amount}
nanobank-transaction-counterparty = {$name} #{$number}
nanobank-category-purchase = Purchase
nanobank-category-deposit = Deposit
nanobank-transaction-tooltip-in = You received N$ {$amount} from {$name} (#{$number}) for "{$reason}"
nanobank-transaction-tooltip-out = You sent N$ {$amount} to {$name} (#{$number}) for "{$reason}"

transfer-funds-button-title = Transfer Funds

#transaction window
transaction-window-title = Transfer Funds
atm-recipient-transfer-number = Recipient:
atm-transfer-amount = Amount:
atm-transfer-reason = Reason:
atm-transfer-reason-charcount = {$count} Characters Remaining
transaction-low-funds = Error : Not Enough Funds
transaction-no-recipient = Error : Recipient Does Not Exist
atm-cancel-button-label = Cancel
atm-confirm-button-label = Confirm
atm-really-confirm-label = Really Confirm

#pos system
pos-window-title = Point-of-sale system
pos-begin-setup-text = This device has not been set up, please press the button below to begin
pos-begin-setup-present-card = This device has not been set up, please enter a valid account number or present a valid bank card to continue
pos-begin-setup-button-text = Begin Setup
pos-setup-title = Merchant Setup
pos-setup-merchant-name = Merchant Name
pos-setup-merchant-name-placeholder = e.g. Fête de la Vanille
pos-setup-recipient-account-number = Recipient Account Number
pos-setup-charge-amount = Charge Amount
pos-setup-reason = Reason
pos-setup-err-invalid-recipient = Error : invalid recipient
pos-setup-err-invalid-transfer-amount = Error : please specify charge amount
pos-setup-confirmed = settings updated!
pos-clear-setup-button-label = Clear Setup
pos-confirm-setup-button-label = Confirm

pos-payment-present-card = Present your bank card to pay
pos-merchant-button = Merchant settings

#merchant lock
pos-lock-title = Merchant Login
pos-lock-prompt = Enter your bank PIN on the keypad to access merchant settings
pos-lock-present-card = Hold your bank card to claim this terminal
pos-lock-invalid = Incorrect PIN
pos-payment-review-title = Review Purchase
pos-payment-name-and-number = {$name} (#{$number})
pos-payment-is-trying-to-charge = Is trying to charge you
pos-payment-scrip-amount = N${$amount}
pos-payment-subtotal = Subtotal
pos-payment-tax-label = Tax ({$percent}%)
pos-payment-total = Total
pos-payment-reason = for "{$reason}"
pos-payment-confirm-button-label = Confirm Transaction
pos-payment-cancel-button-label = Cancel
pos-tax-reason = Sales Tax
pos-tip-reason = Tip

#tip menu
pos-tip-window-title = Add a Tip?
pos-tip-select = Select Tip
pos-tip-preview = N${$amount}
pos-tip-15 = 15% - Good!
pos-tip-18 = 18% - Great!
pos-tip-20 = 20% - Wow!
pos-tip-25 = 25% - Best Service!
pos-tip-custom = Custom Amount
pos-tip-custom-confirm = Tip
pos-tip-none = No Tip
pos-tip-pretax = Tip is calculated before tax

#account management console
account-management-window-title = Payment Records Computer
account-management-access-denied = Access denied.
nanobank-station-balance = Station Balance
nanobank-next-cycle = Next Cycle
nanobank-account-records = Account Records
nanobank-search-placeholder = Search name or number...
nanobank-tab-accounts = Accounts
nanobank-tab-departments = Departments
nanobank-suspend-dept-button = Suspend
nanobank-resume-dept-button = Resume
nanobank-status-label = Status
nanobank-status-eligible = Eligible
nanobank-status-suspended = Suspended
nanobank-set-suspended-button = Suspend Payments
nanobank-set-eligible-button = Resume Payments
nanobank-reason-placeholder = Reason
nanobank-current-pay-label = Current Pay
nanobank-pay-per-cycle = {$amount} Scr/c
nanobank-set-pay-button = Set New Pay
nanobank-grant-bonus-label = Grant Bonus
nanobank-grant-bonus-button = Grant Bonus
nanobank-input-placeholder = input
nanobank-unknown-account = Unknown
nanobank-placeholder = ---
nanobank-station-bank = Station Bank
nanobank-bonus-reason = Bonus
nanobank-cash = Cash
nanobank-deposit-reason = Deposit
nanobank-withdrawal-reason = Withdrawal
nanobank-exchange-tax-name = Currency Exchange
nanobank-exchange-tax-reason = Exchange Tax
nanobank-scrip-cashout-reason = Cashed Out
nanobank-salary-reason = Salary
nanobank-withheld = Withheld
nanobank-withheld-reason = Withheld: {$reason}
nanobank-withheld-wanted = Wanted
nanobank-withheld-detained = Detained
nanobank-payout-announcement = Pay is out! Check your salaries for correct amounts.
nanobank-payout-sender = Station

#currency exchange
currency-exchange-window-title = Currency Exchange
currency-exchange-slot-name = Cash
currency-exchange-title = Spesos {"<->"} Scrip
currency-exchange-rate = Conversion tax: {$tax}%
currency-exchange-empty = Insert spesos or scrip to begin.
currency-exchange-inserted = Inserted: {$count} {$currency}
currency-exchange-preview = You get: {$amount} {$currency}
currency-exchange-spesos = spesos
currency-exchange-scrip = scrip
currency-exchange-insert-button = Insert Cash
currency-exchange-eject-button = Eject
currency-exchange-convert-button = Convert

#card programming
nanobank-program-card-button = Program Card
nanobank-program-card-title = Program Bank Card
nanobank-back-button = Back
nanobank-write-card-button = Write Account To Card
nanobank-card-target = Writing: {$name}
nanobank-no-account-selected = no account selected
nanobank-card-slot-empty = No card inserted
nanobank-card-slot-filled = Card inserted

#new business account
nanobank-new-account-title = New Business Account
nanobank-new-account-subtext = Create a business account for an entrepreneurial tider.
nanobank-new-account-placeholder = Business name
nanobank-new-account-button = Create Account

#scrip cash-out
nanobank-cash-out-button = Cash Out
nanobank-cash-out-title = Cash Out Station Scrip
nanobank-cash-out-balance = Station scrip: {$balance}
nanobank-cash-out-rate = Rate: {$rate} scrip per speso
nanobank-cash-out-confirm = Convert
nanobank-cash-out-preview = Station gets: {$amount} spesos
