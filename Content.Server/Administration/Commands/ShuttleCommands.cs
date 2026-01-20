// SPDX-FileCopyrightText: 2025 Samuka
// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-License-Identifier: MIT

using Content.Server.RoundEnd;
using Content.Shared.Administration;
using Content.Shared.Localizations;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
    [AdminCommand(AdminFlags.Round)]
    public sealed class CallShuttleCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;

        public override string Command => "callshuttle";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (args.Length == 1 && TimeSpan.TryParseExact(args[0], ContentLocalizationManager.TimeSpanMinutesFormats, LocalizationManager.DefaultCulture, out var timeSpan))
                _roundEndSystem.RequestRoundEnd(timeSpan, shell.Player?.AttachedEntity, false);

            else if (args.Length == 1)
                shell.WriteLine(Loc.GetString("shell-timespan-minutes-must-be-correct"));

            else
                _roundEndSystem.RequestRoundEnd(shell.Player?.AttachedEntity, false);
        }
    }

    [AdminCommand(AdminFlags.Round)]
    public sealed class RecallShuttleCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;

        public override string Command => "recallshuttle";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            _roundEndSystem.CancelRoundEndCountdown(shell.Player?.AttachedEntity, forceRecall: true);
        }
    }
}
