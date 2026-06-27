using Robust.Shared.GameStates;

namespace Content.Shared._Impstation.PersonalEconomy.Components;

/// <summary>
/// Controls things for automatic payroll.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StationPayrollComponent : Component
{
    /// <summary>
    /// How long between payout cycles.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan PayoutInterval = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Time till next payout.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan NextPayout;

    /// <summary>
    /// The station has its own scrip account, and that is where people get paid from. 0 until its made.
    /// </summary>
    [DataField, AutoNetworkedField]
    public AccountNumber StationAccount;

    /// <summary>
    /// The base amount of scrip added per cycle.
    /// </summary>
    [DataField]
    public int BaseFunding = 30000;

    /// <summary>
    /// How much scrip to add per crew member.
    /// </summary>
    [DataField]
    public int PerCrewFunding = 1000;

    /// <summary>
    /// Delay so that crew can spawn to get the first payroll.
    /// </summary>
    [DataField]
    public TimeSpan InitialFundDelay = TimeSpan.FromSeconds(10);

    /// <summary>
    /// When the one time roundstart funding should fire.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan InitialFundTime;

    /// <summary>
    /// Whether the one time roundstart funding has already happened.
    /// </summary>
    [DataField]
    public bool InitialFunded;

    /// <summary>
    /// Bool showing if the station account was created or not.
    /// </summary>
    [DataField]
    public bool Initialized;
}
