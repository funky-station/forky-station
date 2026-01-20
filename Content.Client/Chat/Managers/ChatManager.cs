// SPDX-FileCopyrightText: 2025 beck-thompson
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Michael Phillips
// SPDX-FileCopyrightText: 2022 Morbo
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Clyybber
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 ike709
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-FileCopyrightText: 2021 AJCM-git
// SPDX-FileCopyrightText: 2021 Leo
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 Clement-O
// SPDX-FileCopyrightText: 2020 Hugo Laloge
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2020 ShadowCommander
// SPDX-FileCopyrightText: 2019 tentekal
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Client.Administration.Managers;
using Content.Client.Ghost;
using Content.Shared.Administration;
using Content.Shared.Chat;
using Robust.Client.Console;
using Robust.Shared.Utility;

namespace Content.Client.Chat.Managers;

internal sealed class ChatManager : IChatManager
{
    [Dependency] private readonly IClientConsoleHost _consoleHost = default!;
    [Dependency] private readonly IClientAdminManager _adminMgr = default!;
    [Dependency] private readonly IEntitySystemManager _systems = default!;

    private ISawmill _sawmill = default!;

    public void Initialize()
    {
        _sawmill = Logger.GetSawmill("chat");
        _sawmill.Level = LogLevel.Info;
    }

    public void SendAdminAlert(string message)
    {
        // See server-side manager. This just exists for shared code.
    }

    public void SendAdminAlert(EntityUid player, string message)
    {
        // See server-side manager. This just exists for shared code.
    }

    public void SendAdminAlertNoFormatOrEscape(string message)
    {
        // See server-side manager. This just exists for shared code.
    }

    public void SendMessage(string text, ChatSelectChannel channel)
    {
        var str = text.ToString();
        switch (channel)
        {
            case ChatSelectChannel.Console:
                // run locally
                _consoleHost.ExecuteCommand(text);
                break;

            case ChatSelectChannel.LOOC:
                _consoleHost.ExecuteCommand($"looc \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.OOC:
                _consoleHost.ExecuteCommand($"ooc \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Admin:
                _consoleHost.ExecuteCommand($"asay \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Emotes:
                _consoleHost.ExecuteCommand($"me \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Dead:
                if (_systems.GetEntitySystemOrNull<GhostSystem>() is {IsGhost: true})
                    goto case ChatSelectChannel.Local;

                if (_adminMgr.HasFlag(AdminFlags.Admin))
                    _consoleHost.ExecuteCommand($"dsay \"{CommandParsing.Escape(str)}\"");
                else
                    _sawmill.Warning("Tried to speak on deadchat without being ghost or admin.");
                break;

            // TODO sepearate radio and say into separate commands.
            case ChatSelectChannel.Radio:
            case ChatSelectChannel.Local:
                _consoleHost.ExecuteCommand($"say \"{CommandParsing.Escape(str)}\"");
                break;

            case ChatSelectChannel.Whisper:
                _consoleHost.ExecuteCommand($"whisper \"{CommandParsing.Escape(str)}\"");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
        }
    }
}
