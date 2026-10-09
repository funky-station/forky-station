using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared._Impstation.PersonalEconomy;

/// <summary>
/// gives jobs a salary. if a job is listed multiple times, it stacks!
/// </summary>
[Prototype("paymentSalary")]
public sealed partial class PaymentSalaryPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// roles this applies for
    /// </summary>
    [DataField]
    public List<ProtoId<JobPrototype>> Roles = [];

    [DataField]
    public int Salary;
}
