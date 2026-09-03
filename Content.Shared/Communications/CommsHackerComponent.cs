using Content.Shared.Random;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
///<funky change>
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Radio;
//</funky change>

namespace Content.Shared.Communications;

/// <summary>
/// Component for hacking a communications console to call in a threat.
/// Can only be done once, the component is remove afterwards.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedCommsHackerSystem))]
public sealed partial class CommsHackerComponent : Component
{
    /// <summary>
    /// Time taken to hack the console
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan Delay = TimeSpan.FromSeconds(20);

    /// <summary>
    /// Weighted random for the possible threats to choose from.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<WeightedRandomPrototype> Threats = string.Empty;

    /// <summary>
    /// The radio channel for security
    /// </summary>
    /// funky change for early Ninja warning
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

/// <summary>
/// A threat that can be called in to the station by a ninja hacking a communications console.
/// Generally some kind of mid-round minor antag, though you could make it call in scrubber backflow if you wanted to.
/// You wouldn't do that, right?
/// </summary>
[Prototype]
public sealed partial class NinjaHackingThreatPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Locale id for the announcement to be made from CentCom.
    /// </summary>
    [DataField(required: true)]
    public LocId Announcement;

    /// <summary>
    /// The game rule for the threat to be added, it should be able to work when added mid-round otherwise this will do nothing.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId Rule;
}
