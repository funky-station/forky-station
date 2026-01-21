// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Moony <moonheart08@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Jesse Rougeau <jmaster9999@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Ygg01 <y.laughing.man.y@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Matthew Herber <32679887+Happyrobot33@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.GameTicking.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System.Linq;

namespace Content.Server.GameTicking;

public sealed partial class GameTicker
{
    [ViewVariables]
    public ProtoId<LobbyBackgroundPrototype>? LobbyBackground { get; private set; }

    [ViewVariables]
    private List<ProtoId<LobbyBackgroundPrototype>>? _lobbyBackgrounds;

    private static readonly string[] WhitelistedBackgroundExtensions = new string[] {"png", "jpg", "jpeg", "webp"};

    private void InitializeLobbyBackground()
    {
        var allprotos = _prototypeManager.EnumeratePrototypes<LobbyBackgroundPrototype>().ToList();
        _lobbyBackgrounds ??= new List<ProtoId<LobbyBackgroundPrototype>>();

        //create protoids from them
        foreach (var proto in allprotos)
        {
            var ext = proto.Background.Extension;
            if (!WhitelistedBackgroundExtensions.Contains(ext))
                continue;

            //create a protoid and add it to the list
            _lobbyBackgrounds.Add(new ProtoId<LobbyBackgroundPrototype>(proto.ID));
        }

        RandomizeLobbyBackground();
    }

    private void RandomizeLobbyBackground()
    {
        if (_lobbyBackgrounds != null && _lobbyBackgrounds.Count != 0)
            LobbyBackground = _robustRandom.Pick(_lobbyBackgrounds);
        else
            LobbyBackground = null;
    }
}
