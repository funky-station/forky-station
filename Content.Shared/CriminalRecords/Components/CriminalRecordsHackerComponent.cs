using Content.Shared.CriminalRecords.Systems;
using Content.Shared.Dataset;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
///<funky change>
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Radio;
//</funky change>

namespace Content.Shared.CriminalRecords.Components;

/// <summary>
/// Lets the user hack a criminal records console, once.
/// Everyone is set to wanted with a randomly picked reason.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedCriminalRecordsHackerSystem))]
public sealed partial class CriminalRecordsHackerComponent : Component
{
    /// <summary>
    /// How long the doafter is for hacking it.
    /// </summary>
    public TimeSpan Delay = TimeSpan.FromSeconds(20);

    /// <summary>
    /// Dataset of random reasons to use.
    /// </summary>
    [DataField]
    public ProtoId<LocalizedDatasetPrototype> Reasons = "CriminalRecordsWantedReasonPlaceholders";

    /// <summary>
    /// Announcement made after the console is hacked.
    /// </summary>
    [DataField]
    public LocId Announcement = "ninja-criminal-records-hack-announcement";

    /// <summary>
    /// The radio channel for security
    /// </summary>
    /// funky change for Ninja warning
    [DataField("securityChannel", customTypeSerializer: typeof(ProtoId<RadioChannelPrototype>))]

    public string SecurityChannel = "Security";
    /// <summary>
    /// Minimum time between sending the security warning radio message for a ninja hacking attempt
    /// </summary>
    /// funky, prevents spamming security with messages
    [DataField]
    public TimeSpan WarningCooldown = TimeSpan.FromSeconds(10);

    /// <summary>
    /// The next time the warning radio message may be sent
    /// </summary>
    /// funky
    [DataField]
    public TimeSpan NextWarningTime = TimeSpan.Zero;
}
