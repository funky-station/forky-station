using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Impstation.PersonalEconomy.Components;

/// <summary>
/// This is used for keeping track of the data of one bank account
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BankAccountComponent : Component
{
    [DataField, AutoNetworkedField]
    public string Name = string.Empty;

    [DataField, AutoNetworkedField]
    public AccountNumber AccountNumber;

    // the pin should never be replicated to all clients
    [DataField]
    public Pin Pin;

    // store user id to tell proper player their pin
    public NetUserId? OwnerUserId;

    [DataField, AutoNetworkedField]
    public int Balance = 0;
    [DataField, AutoNetworkedField]
    public int Salary = 0;

    // whether this account gets paid out on the payroll cycle. suspended accounts are skipped
    [DataField, AutoNetworkedField]
    public PaymentStatus Status = PaymentStatus.Eligible;
    [DataField, AutoNetworkedField]
    public string StatusReason = string.Empty;

    // department proto id this account belongs to.
    [DataField, AutoNetworkedField]
    public string Department = string.Empty;

    // command roles are skipped by department-wide pay suspension unless command itself is targeted
    [DataField, AutoNetworkedField]
    public bool IsCommand;

    // station record id of the owner, used to auto-withhold pay for wanted/detained crew
    [DataField]
    public uint StationRecordId;

    // true for the station's scrip-pool account; hidden from the management console crew list
    [DataField, AutoNetworkedField]
    public bool IsStationAccount;

    //ok so all of this rambling is irrelevant now that I've ent-ified things, but I'll leave it here as a testament to 1 am me's thought process :)
    //make this a server-only property in the comp, then have the data get sent back to the client via a BUI state?
    //or just have it send a normal message back to the client system that then caches the values?
    //or just do a field delta state for this field specifically?
    //or ent-ify bank accounts? it'll be like 50 ents with 3 comps that get force-PVS'd to everyone which feels kinda sussy but it does massively reduce the size of each state
    //basically I don't want to be replicating every transaction to every player all the time because that will potentially be a lot of network traffic
    //ok yeah I think I've convinced myself that ent-ifying bank accounts is the way to go?
    //I'll only store 10 transactions, also this lets everything be in shared, so everything seems good?
    [DataField, AutoNetworkedField]
    public List<BankTransaction> Transactions = [];
}

[Serializable, NetSerializable]
public enum PaymentStatus : byte
{
    Eligible,
    Suspended,
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class BankTransaction
{
    public BankTransaction(AccountNumber other, string name, int amount, double timestamp, string reason)
    {
        OtherAccount = other;
        Name = name;
        Amount = amount;
        Timestamp = timestamp;
        Reason = reason;
    }

    [DataField] public AccountNumber OtherAccount = 0;
    [DataField] public string Name = ""; //technically this doesn't need to be stored here since both the client & server will know what all account names are at all times, but I like it keeping track of renambed accounts for scams and such
    [DataField] public int Amount = 0;
    [DataField] public double Timestamp = 0;
    [DataField] public string Reason = string.Empty; //limited to 64 chars
}

/// <summary>
/// Used to enforce transfer number / access number constraints. technically I think they can just be freely casted between because they can both go to and from int but this makes me feel smart.
/// </summary>
[DataDefinition, Serializable, NetSerializable]
public partial struct AccountNumber : IEquatable<AccountNumber>
{
    public readonly int Number;

    public AccountNumber(int number)
    {
        Number = number;
    }

    public static implicit operator int(AccountNumber number)
    {
        return number.Number;
    }

    public static implicit operator AccountNumber(int number)
    {
        return new AccountNumber(number);
    }

    public bool Equals(AccountNumber other)
    {
        return Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        return obj is AccountNumber other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Number;
    }
}

[DataDefinition, Serializable, NetSerializable]
public partial struct Pin : IEquatable<Pin>
{
    public readonly int Number;

    public Pin(int number)
    {
        Number = number;
    }

    public static implicit operator int(Pin number)
    {
        return number.Number;
    }

    public static implicit operator Pin(int number)
    {
        return new Pin(number);
    }

    public bool Equals(Pin other)
    {
        return Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        return obj is Pin other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Number;
    }
}
