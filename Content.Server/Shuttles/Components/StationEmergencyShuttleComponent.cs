using Content.Server.Shuttles.Systems;
using Content.Shared._MACRO.Announcements;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;

namespace Content.Server.Shuttles.Components;

/// <summary>
/// This is used for controlling evacuation for a station.
/// </summary>
[RegisterComponent]
public sealed partial class StationEmergencyShuttleComponent : Component
{
    /// <summary>
    /// The emergency shuttle assigned to this station.
    /// </summary>
    [DataField, Access(typeof(ShuttleSystem), typeof(EmergencyShuttleSystem), Friend = AccessPermissions.ReadWrite)]
    public EntityUid? EmergencyShuttle;

    /// <summary>
    /// Emergency shuttle map path for this station.
    /// </summary>
    [DataField("emergencyShuttlePath", customTypeSerializer: typeof(ResPathSerializer))]
    public ResPath EmergencyShuttlePath { get; set; } = new("/Maps/Shuttles/emergency.yml");

    /// <summary>
    /// The announcement made when the shuttle has successfully docked with the station.
    /// </summary>
    [DataField]
    public LocId DockedAnnouncement = "emergency-shuttle-docked";

    /// <summary>
    /// Sound played when the shuttle has successfully docked with the station.
    /// </summary>
    [DataField]
    public ProtoId<AnnouncementSoundPrototype> DockedAudio = "ShuttleDock"; // Macrocosm - announcement prototypes

    /// <summary>
    /// The announcement made when the shuttle is unable to dock and instead parks in nearby space.
    /// </summary>
    [DataField]
    public LocId NearbyAnnouncement = "emergency-shuttle-nearby";

    /// <summary>
    /// Sound played when the shuttle is unable to dock and instead parks in nearby space.
    /// </summary>
    [DataField]
    public ProtoId<AnnouncementSoundPrototype> NearbyAudio = "Notice1"; // Macrocosm - announcement prototypes

    /// <summary>
    /// The announcement made when the shuttle is unable to find a station.
    /// </summary>
    [DataField]
    public LocId FailureAnnouncement = "emergency-shuttle-good-luck";

    /// <summary>
    /// Sound played when the shuttle is unable to find a station.
    /// </summary>
    [DataField]
    public ProtoId<AnnouncementSoundPrototype> FailureAudio = "Notice1"; // Macrocosm - announcement prototypes

    /// <summary>
    /// Text appended to the docking announcement if the launch time has been extended.
    /// </summary>
    [DataField]
    public LocId LaunchExtendedMessage = "emergency-shuttle-extended";
}
