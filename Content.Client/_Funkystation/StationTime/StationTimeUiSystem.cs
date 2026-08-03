using Content.Shared._Funkystation.StationTime.Components;
using Content.Shared._Funkystation.StationTime.EntitySystems;
using Content.Shared.Station;
using Robust.Client.Player;

namespace Content.Client._Funkystation.StationTime;

public sealed partial class StationTimeUiSystem : EntitySystem
{
    [Dependency] private IPlayerManager _player = null!;
    [Dependency] private SharedStationSystem _station = null!;
    [Dependency] private StationTimeSystem _stationTime = null!;

    /// <summary>
    /// gets the formatted station date and time
    /// </summary>
    public string GetPdaTimeString()
    {
        var uid = _player.LocalSession?.AttachedEntity;

        if (uid != null)
        {
            var station = _station.GetOwningStation(uid.Value);
            if (station != null && TryComp<StationTimeComponent>(station.Value, out var timeComp))
            {
                var time = _stationTime.GetStationTime((station.Value, timeComp));
                return _stationTime.FormatTimestamp(time);
            }
        }

        return "[color=red]ERR: NO SIGNAL[/color]";
    }
}
