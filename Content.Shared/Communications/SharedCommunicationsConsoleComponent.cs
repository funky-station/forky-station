using Content.Shared.AlertLevel;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications
{
    [Virtual]
    public partial class SharedCommunicationsConsoleComponent : Component
    {
    }

    [Serializable, NetSerializable]
    public sealed class CommunicationsConsoleInterfaceState : BoundUserInterfaceState
    {
        public readonly bool CanAnnounce;
        public readonly bool CanBroadcast = true;
        public readonly bool CanCall;
        public readonly TimeSpan? ExpectedCountdownEnd;
        public readonly bool CountdownStarted;
        public readonly bool CanBypassPA; // funky


        public CommunicationsConsoleInterfaceState(
            bool canAnnounce,
            bool canCall,
            TimeSpan? expectedCountdownEnd = null,
            bool canBypassPA = false) // funky
        {
            CanAnnounce = canAnnounce;
            CanCall = canCall;
            ExpectedCountdownEnd = expectedCountdownEnd;
            CountdownStarted = expectedCountdownEnd != null;
            CanBypassPA = canBypassPA; // funky
        }
    }

    [Serializable, NetSerializable]
    public sealed class CommunicationsConsoleSelectAlertLevelMessage(ProtoId<AlertLevelPrototype> level) : BoundUserInterfaceMessage
    {
        public ProtoId<AlertLevelPrototype> Level = level;
    }

    [Serializable, NetSerializable]
    public sealed class CommunicationsConsoleAnnounceMessage : BoundUserInterfaceMessage
    {
        public readonly string Message;
        public readonly bool BypassPA; //funky

        public CommunicationsConsoleAnnounceMessage(string message, bool bypassPA = false) // funky - add pa bypass option
        {
            Message = message;
            BypassPA = bypassPA; // funky
        }
    }

    [Serializable, NetSerializable]
    public sealed class CommunicationsConsoleBroadcastMessage : BoundUserInterfaceMessage
    {
        public readonly string Message;
        public CommunicationsConsoleBroadcastMessage(string message)
        {
            Message = message;
        }
    }

    [Serializable, NetSerializable]
    public sealed class CommunicationsConsoleCallEmergencyShuttleMessage : BoundUserInterfaceMessage
    {
    }

    [Serializable, NetSerializable]
    public sealed class CommunicationsConsoleRecallEmergencyShuttleMessage : BoundUserInterfaceMessage
    {
    }

    [Serializable, NetSerializable]
    public enum CommunicationsConsoleUiKey
    {
        Key
    }
}
