// SPDX-FileCopyrightText: 2024 PrPleGoo
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Paul
// SPDX-FileCopyrightText: 2022 E F R
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-License-Identifier: MIT

using Content.Client.UserInterface.Systems.Bwoink;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.Network;

namespace Content.Client.Commands;

[AnyCommand]
public sealed class OpenAHelpCommand : LocalizedCommands
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = default!;

    public override string Command => "openahelp";

    public override string Help => LocalizationManager.GetString($"cmd-{Command}-help", ("command", Command));

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length >= 2)
        {
            shell.WriteLine(Help);
            return;
        }
        if (args.Length == 0)
        {
            _userInterfaceManager.GetUIController<AHelpUIController>().Open();
        }
        else
        {
            if (Guid.TryParse(args[0], out var guid))
            {
                var targetUser = new NetUserId(guid);
                _userInterfaceManager.GetUIController<AHelpUIController>().Open(targetUser);
            }
            else
            {
                shell.WriteError(LocalizationManager.GetString($"cmd-{Command}-error"));
            }
        }
    }
}
