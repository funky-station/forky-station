// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Rainfey <rainfey0+github@gmail.com>
// SPDX-FileCopyrightText: 2025 Milon <milonpl.git@proton.me>
// SPDX-License-Identifier: MIT

using Content.Server.Antag;
using Content.Server.Traitor.Components;
using Content.Shared.Mind.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Traitor.Systems;

/// <summary>
/// Makes entities with <see cref="AutoTraitorComponent"/> a traitor either immediately if they have a mind or when a mind is added.
/// </summary>
public sealed class AutoTraitorSystem : EntitySystem
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly ISharedPlayerManager _player = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AutoTraitorComponent, MindAddedMessage>(OnMindAdded);
    }

    private void OnMindAdded(EntityUid uid, AutoTraitorComponent comp, MindAddedMessage args)
    {
        if (!_player.TryGetSessionById(args.Mind.Comp.UserId, out var session))
            return;

        _antag.ForceMakeAntag<AutoTraitorComponent>(session, comp.Profile);
    }
}
