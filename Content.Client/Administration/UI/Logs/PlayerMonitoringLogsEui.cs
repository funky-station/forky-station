// SPDX-FileCopyrightText: 2026 Space Wizards Federation
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Client.Eui;
using Content.Shared.Administration.Logs;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using static Content.Shared.Administration.Logs.PlayerMonitoringLogsEuiMsg;

namespace Content.Client.Administration.UI.Logs;

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
        _window.DenominatorOption.Clear();
        _window.DenominatorOption.AddItem(Loc.GetString("player-monitoring-denom-flagged"), id: (int)PlayerMonitoringDenominatorMode.FlaggedRounds);
        _window.DenominatorOption.AddItem(Loc.GetString("player-monitoring-denom-played"), id: (int)PlayerMonitoringDenominatorMode.RoundsPlayed);
        _window.DenominatorOption.SelectId((int)s.DefaultDenominatorMode);
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

        var mode = (PlayerMonitoringDenominatorMode)_window.DenominatorOption.SelectedId;
        SendMessage(new RequestQuery
        {
            UserNameExact = _window.UserNameEdit.Text,
            Days = _window.DaysSpin.Value,
            DenominatorMode = mode
        });
    }

    public override void Closed()
    {
        base.Closed();
        _window?.Dispose();
        _window = null;
    }
}
