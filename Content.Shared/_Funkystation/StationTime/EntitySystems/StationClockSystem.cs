using Content.Shared._Funkystation.StationTime.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Station;
using Robust.Shared.Audio.Systems;
using Content.Shared.Audio;

namespace Content.Shared._Funkystation.StationTime.EntitySystems;

public sealed partial class StationClockSystem : EntitySystem
{
    [Dependency] private StationTimeSystem _stationTime = null!;
    [Dependency] private SharedStationSystem _station = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedAmbientSoundSystem _ambient = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationClockComponent, ExaminedEvent>(OnExamined);
        SubscribeLocalEvent<StationClockComponent, ActivateInWorldEvent>(OnActivated);
    }

    private void OnActivated(EntityUid uid, StationClockComponent comp, ActivateInWorldEvent args)
    {
        if (args.Handled)
            return;

        comp.Enabled = !comp.Enabled;
        Dirty(uid, comp);

        if (TryComp<AmbientSoundComponent>(uid, out var ambient))
        {
            _ambient.SetAmbience(uid, comp.Enabled, ambient);
        }

        _audio.PlayPredicted(comp.ToggleSound, uid, args.User);

        var ev = new StationClockToggledEvent(comp.Enabled);
        RaiseLocalEvent(uid, ref ev);

        args.Handled = true;
    }

    private void OnExamined(EntityUid uid, StationClockComponent comp, ExaminedEvent args)
    {
        if (!comp.Enabled)
        {
            args.PushText(Loc.GetString("station-clock-examine-off"));
            return;
        }

        var station = _station.GetOwningStation(uid);
        if (station is not { } stationEnt || !TryComp<StationTimeComponent>(stationEnt, out var timeComp))
        {
            args.PushText(Loc.GetString("station-clock-examine-broken"));
            return;
        }

        var time = _stationTime.GetStationTime((stationEnt, timeComp));

        args.PushMarkup(comp.ShowDate
                ? Loc.GetString("station-clock-examine-datetime",
                    ("date", _stationTime.FormatDate(time)),
                    ("time", _stationTime.FormatTime(time, comp.ShowSeconds)))
                : Loc.GetString("station-clock-examine-time",
                    ("time", _stationTime.FormatTime(time, comp.ShowSeconds))),
            priority: 1);

        if (!comp.ShowTemperature)
            return;

        args.PushMarkup(comp.LastTemperatureCelsius is { } c
                ? Loc.GetString("station-clock-examine-temperature", ("temp", $"{c:F1}"))
                : Loc.GetString("station-clock-examine-temperature-unknown"),
            priority: 0);
    }
}
