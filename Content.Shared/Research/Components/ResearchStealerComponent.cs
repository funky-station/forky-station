using Content.Shared.Research.Systems;
using Robust.Shared.GameStates;
///<funky change>
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared.Radio;
using Robust.Shared.Prototypes;
//</funky change>

namespace Content.Shared.Research.Components;

/// <summary>
/// Component for stealing technologies from a R&D server, when gloves are enabled.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedResearchStealerSystem))]
public sealed partial class ResearchStealerComponent : Component
{
    /// <summary>
    /// Time taken to steal research from a server
    /// </summary>
    [DataField("delay"), ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan Delay = TimeSpan.FromSeconds(20);

    /// <summary>
    /// The minimum number of technologies that will be stolen
    /// </summary>
    [DataField]
    public int MinToSteal = 4;

    /// <summary>
    /// The maximum number of technologies that will be stolen
    /// </summary>
    [DataField]
    public int MaxToSteal = 8;

    /// <summary>
    /// The radio channel for science
    /// </summary>
    /// funky
    [DataField("scienceChannel", customTypeSerializer: typeof(ProtoId<RadioChannelPrototype>))]
    public string ScienceChannel = "Science";

    /// <summary>
    /// Minimum time between sending the science warning radio message
    /// </summary>
    /// funky, prevents spamming science with messages
    [DataField]
    public TimeSpan WarningCooldown = TimeSpan.FromSeconds(10);

    /// <summary>
    /// The next time the warning radio message may be sent
    /// </summary>
    /// funky
    [DataField]
    public TimeSpan NextWarningTime = TimeSpan.Zero;
}
