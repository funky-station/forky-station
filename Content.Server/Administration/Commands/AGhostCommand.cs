// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2025 Milon
// SPDX-FileCopyrightText: 2025 J
// SPDX-FileCopyrightText: 2024 lzk
// SPDX-FileCopyrightText: 2024 Simon
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-FileCopyrightText: 2022 Jacob Tong
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Injazz
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 NuclearWinter
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2018 PJB3005
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Server.GameTicking;
using Content.Server.Ghost;
using Content.Server.Mind;
using Content.Shared.Administration;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Player;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class AGhostCommand : LocalizedCommands
{
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;

    public override string Command => "aghost";
    public override string Help => "aghost";

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var names = _playerManager.Sessions.OrderBy(c => c.Name).Select(c => c.Name).ToArray();
            return CompletionResult.FromHintOptions(names, LocalizationManager.GetString("shell-argument-username-optional-hint"));
        }

        return CompletionResult.Empty;
    }

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length > 1)
        {
            shell.WriteError(LocalizationManager.GetString("shell-wrong-arguments-number"));
            return;
        }

        var player = shell.Player;
        var self = player != null;
        if (player == null)
        {
            // If you are not a player, you require a player argument.
            if (args.Length == 0)
            {
                shell.WriteError(LocalizationManager.GetString("shell-need-exactly-one-argument"));
                return;
            }

            var didFind = _playerManager.TryGetSessionByUsername(args[0], out player);
            if (!didFind)
            {
                shell.WriteError(LocalizationManager.GetString("shell-target-player-does-not-exist"));
                return;
            }
        }

        // If you are a player and a username is provided, a lookup is done to find the target player.
        if (args.Length == 1)
        {
            var didFind = _playerManager.TryGetSessionByUsername(args[0], out player);
            if (!didFind)
            {
                shell.WriteError(LocalizationManager.GetString("shell-target-player-does-not-exist"));
                return;
            }
        }

        var mindSystem = _entities.System<SharedMindSystem>();
        var metaDataSystem = _entities.System<MetaDataSystem>();
        var ghostSystem = _entities.System<SharedGhostSystem>();
        var transformSystem = _entities.System<TransformSystem>();
        var gameTicker = _entities.System<GameTicker>();

        if (!mindSystem.TryGetMind(player, out var mindId, out var mind))
        {
            shell.WriteError(self
                ? LocalizationManager.GetString("aghost-no-mind-self")
                : LocalizationManager.GetString("aghost-no-mind-other"));
            return;
        }

        if (mind.VisitingEntity != default && _entities.TryGetComponent<GhostComponent>(mind.VisitingEntity, out var oldGhostComponent))
        {
            mindSystem.UnVisit(mindId, mind);
            // If already an admin ghost, then return to body.
            if (oldGhostComponent.CanGhostInteract)
                return;
        }

        var canReturn = mind.CurrentEntity != null
                        && !_entities.HasComponent<GhostComponent>(mind.CurrentEntity);
        var coordinates = player!.AttachedEntity != null
            ? _entities.GetComponent<TransformComponent>(player.AttachedEntity.Value).Coordinates
            : gameTicker.GetObserverSpawnPoint();
        var ghost = _entities.SpawnEntity(GameTicker.AdminObserverPrototypeName, coordinates);
        transformSystem.AttachToGridOrMap(ghost, _entities.GetComponent<TransformComponent>(ghost));

        if (canReturn)
        {
            // TODO: Remove duplication between all this and "GamePreset.OnGhostAttempt()"...
            if (!string.IsNullOrWhiteSpace(mind.CharacterName))
                metaDataSystem.SetEntityName(ghost, mind.CharacterName);
            else if (!string.IsNullOrWhiteSpace(player.Name))
                metaDataSystem.SetEntityName(ghost, player.Name);

            mindSystem.Visit(mindId, ghost, mind);
        }
        else
        {
            metaDataSystem.SetEntityName(ghost, player.Name);
            mindSystem.TransferTo(mindId, ghost, mind: mind);
        }

        var comp = _entities.GetComponent<GhostComponent>(ghost);
        ghostSystem.SetCanReturnToBody((ghost, comp), canReturn);
    }
}
