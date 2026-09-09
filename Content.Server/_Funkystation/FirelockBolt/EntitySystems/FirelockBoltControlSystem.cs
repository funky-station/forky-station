using Content.Server.Atmos.Monitor.Systems;
using Content.Server.Doors.Systems;
using Content.Shared._Funkystation.FirelockBolt.Components;
using Content.Shared._Funkystation.FirelockBolt.EntitySystems;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Doors.Components;

namespace Content.Server._Funkystation.FirelockBolt.EntitySystems;

public sealed partial class FirelockBoltControlSystem : SharedFirelockBoltControlSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FirelockBoltControlComponent, AtmosAlarmEvent>(OnAtmosAlarm, before: new[] { typeof(FirelockSystem) });
    }

    private void OnAtmosAlarm(EntityUid uid, FirelockBoltControlComponent component, AtmosAlarmEvent args)
    {
        component.AlarmActive = args.AlarmType != AtmosAlarmType.Normal;
        Dirty(uid, component);

        if (component.Override)
            return;

        if (component.AlarmActive)
        {
            // If an alarm is triggered and the door is closed, bolt
            if (DoorQuery.TryComp(uid, out var door) && (door.State == DoorState.Closed || door.State == DoorState.Welded))
            {
                if (DoorBoltQuery.TryComp(uid, out var bolt))
                    DoorSystem.SetBoltsDown((uid, bolt), true);
            }
        }
        else
        {
            // If the alarm clears, unbolt
            if (DoorBoltQuery.TryComp(uid, out var bolt))
                DoorSystem.SetBoltsDown((uid, bolt), false);
        }

        PushState((uid, component));
    }
}
