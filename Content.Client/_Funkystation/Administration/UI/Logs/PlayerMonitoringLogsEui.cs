using System.Globalization;
using System.Linq;
using Content.Client.Eui;
using Content.Shared._Funkystation.Administration.Logs;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using static Content.Shared._Funkystation.Administration.Logs.PlayerMonitoringLogsEuiMsg;

namespace Content.Client._Funkystation.Administration.UI.Logs;

public sealed class PlayerMonitoringLogsEui : BaseEui
{
    [Dependency] private readonly ILogManager _log = default!;

    private ISawmill _sawmill = default!;
    private PlayerMonitoringLogsWindow? _window;
    private int _maxDays = 90;

    public PlayerMonitoringLogsEui()
    {
        IoCManager.InjectDependencies(this);
        _sawmill = _log.GetSawmill("admin.player_monitoring.ui");
    }

    public override void Opened()
    {
        base.Opened();
        _window = new PlayerMonitoringLogsWindow();
        _window.OnClose += () => SendMessage(new CloseEuiMessage());
        _window.RefreshButton.OnPressed += _ => RequestQuery();
        _window.LoadMoreButton.OnPressed += _ => SendMessage(new NextQuery());
        _window.DaysSpin.InitDefaultButtons();
        _window.OpenCentered();
    }

    public override void HandleState(EuiStateBase state)
    {
        base.HandleState(state);

        if (_window == null || state is not PlayerMonitoringLogsEuiState s)
            return;

        _maxDays = Math.Max(1, s.MaxDays);
        _window.DaysSpin.IsValid = v => v >= 1 && v <= _maxDays;
        _window.DaysSpin.OverrideValue(Math.Clamp(s.DefaultDays, 1, _maxDays));
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (_window == null)
            return;

        switch (msg)
        {
            case QueryResult q:
                if (q.Replace)
                {
                    _window.SummaryContainer.DisposeAllChildren();
                    _window.DetailsContainer.DisposeAllChildren();

                    if (!string.IsNullOrEmpty(q.QueryError))
                    {
                        _window.StatusLabel.Text = Loc.GetString("player-monitoring-query-failed");
                        _window.LoadMoreButton.Disabled = true;
                        return;
                    }

                    if (q.UserNotFound)
                    {
                        _window.StatusLabel.Text = Loc.GetString("player-monitoring-user-not-found");
                        _window.LoadMoreButton.Disabled = true;
                        return;
                    }

                    _window.StatusLabel.Text = Loc.GetString("player-monitoring-status",
                        ("flagged", q.FlaggedRoundsDenominator ?? 0),
                        ("played", q.RoundsPlayedDenominator ?? 0),
                        ("from", q.RangeStartUtc));

                    if (q.Summary != null)
                    {
                        foreach (var (type, entry) in q.Summary.OrderBy(x => x.Key.ToString()))
                        {
                            var line = Loc.GetString("player-monitoring-summary-line",
                                ("type", type.ToString()),
                                ("count", entry.Count),
                                ("drounds", entry.DistinctRounds),
                                ("pf", entry.PercentFlaggedRounds.ToString("0.##")),
                                ("pp", entry.PercentRoundsPlayed.ToString("0.##")));
                            _window.SummaryContainer.AddChild(new Label { Text = line });
                        }
                    }

                    _window.SummaryContainer.AddChild(new Label
                    {
                        Text = Loc.GetString("player-monitoring-daily-play-header"),
                        Margin = new Thickness(0, 8, 0, 2)
                    });

                    if (q.DailyPlayByUtcDay is not { Count: > 0 })
                    {
                        _window.SummaryContainer.AddChild(new Label
                            { Text = Loc.GetString("player-monitoring-daily-play-none") });
                    }
                    else
                    {
                        var inv = CultureInfo.InvariantCulture;
                        _window.SummaryContainer.AddChild(new Label
                        {
                            Text = Loc.GetString("player-monitoring-daily-play-total",
                                ("hours", q.TotalDailyPlaySpanHours.ToString("0.##", inv)))
                        });
                        _window.SummaryContainer.AddChild(new Label
                        {
                            Text = Loc.GetString("player-monitoring-daily-play-avg",
                                ("hours", q.AverageDailyPlaySpanHours.ToString("0.##", inv)),
                                ("days", q.DailyPlayActiveDayCount))
                        });
                        foreach (var day in q.DailyPlayByUtcDay.OrderBy(d => d.UtcDate))
                        {
                            _window.SummaryContainer.AddChild(new Label
                            {
                                Text = Loc.GetString("player-monitoring-daily-play-day-line",
                                    ("date", day.UtcDate),
                                    ("hours", day.SpanHours.ToString("0.##", inv)))
                            });
                        }
                    }
                }

                if (!string.IsNullOrEmpty(q.QueryError) && !q.Replace)
                {
                    _window.StatusLabel.Text = Loc.GetString("player-monitoring-query-failed");
                    _window.LoadMoreButton.Disabled = true;
                    break;
                }

                foreach (var row in q.Rows)
                {
                    var text = Loc.GetString("player-monitoring-detail-line",
                        ("utc", row.Utc),
                        ("round", row.RoundId),
                        ("type", row.EventType.ToString()),
                        ("user", row.DisplayUserName),
                        ("job", row.Job ?? "-"),
                        ("station", row.Station ?? "-"),
                        ("sub", row.SubReason ?? "-"),
                        ("exit", row.ExitKind ?? "-"),
                        ("reason", row.DisconnectReason ?? "-"),
                        ("redial", row.RedialFlag?.ToString() ?? "-"),
                        ("min", row.MinutesInRound?.ToString("0.#") ?? "-"),
                        ("watchedproto", row.WatchedGhostEntityPrototype ?? "-"),
                        ("grname", row.GhostRoleName ?? "-"),
                        ("rndmin", row.MinutesSinceRoundStart?.ToString("0.#") ?? "-"),
                        ("early", row.EarlyLeave ? Loc.GetString("player-monitoring-early-leave-marker") : "-"),
                        ("afkmax", row.AfkMaxIdleMinutes?.ToString("0.#") ?? "-"),
                        ("afkthr", row.AfkThresholdMinutes?.ToString("0.#") ?? "-"));
                    _window.DetailsContainer.AddChild(new Label { Text = text });
                }

                _window.LoadMoreButton.Disabled = !q.HasNext;
                break;
        }
    }

    private void RequestQuery()
    {
        if (_window == null)
            return;

        SendMessage(new RequestQuery
        {
            UserNameExact = _window.UserNameEdit.Text,
            Days = _window.DaysSpin.Value
        });
    }

    public override void Closed()
    {
        base.Closed();
        _window?.Dispose();
        _window = null;
    }
}
