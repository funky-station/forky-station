using Content.Shared._Impstation.PersonalEconomy.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Events;

// all sent from the account management console, targeting an account by access number

[Serializable, NetSerializable]
public sealed class SetAccountStatusMessage(AccountNumber account, PaymentStatus status, string reason) : BoundUserInterfaceMessage
{
    public AccountNumber Account = account;
    public PaymentStatus Status = status;
    public string Reason = reason;
}

[Serializable, NetSerializable]
public sealed class SetAccountSalaryMessage(AccountNumber account, int salary) : BoundUserInterfaceMessage
{
    public AccountNumber Account = account;
    public int Salary = salary;
}

[Serializable, NetSerializable]
public sealed class GrantAccountBonusMessage(AccountNumber account, int amount) : BoundUserInterfaceMessage
{
    public AccountNumber Account = account;
    public int Amount = amount;
}

// writes the given account's details onto the card currently in the console slot
[Serializable, NetSerializable]
public sealed class WriteCardMessage(AccountNumber account) : BoundUserInterfaceMessage
{
    public AccountNumber Account = account;
}

[Serializable, NetSerializable]
public sealed class CreateBusinessAccountMessage(string name) : BoundUserInterfaceMessage
{
    public string Name = name;
}

// cashes the given amount of the station's scrip into physical spesos at the console's rate
[Serializable, NetSerializable]
public sealed class ConvertStationScripMessage(int amount) : BoundUserInterfaceMessage
{
    public int Amount = amount;
}

// bulk actions over every account in a department

[Serializable, NetSerializable]
public sealed class SetDepartmentStatusMessage(string department, PaymentStatus status, string reason) : BoundUserInterfaceMessage
{
    public string Department = department;
    public PaymentStatus Status = status;
    public string Reason = reason;
}

[Serializable, NetSerializable]
public sealed class GrantDepartmentBonusMessage(string department, int amount) : BoundUserInterfaceMessage
{
    public string Department = department;
    public int Amount = amount;
}
