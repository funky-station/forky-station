// SPDX-FileCopyrightText: 2020-2022 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2020 Exp <theexp111@gmail.com>
// SPDX-FileCopyrightText: 2020 nuke <47336974+nuke-makes-games@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021, 2023 metalgearsloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2021 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2022 Chief-Engineer <119664036+Chief-Engineer@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2023 Vordenburg <114301317+Vordenburg@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 0x6273 <0x40@keemail.me>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2026 pathetic meowmeow <uhhadd@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Hands.Systems;
using Content.Shared.Administration;
using Content.Shared.Hands.Components;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.Body.Commands
{
    [AdminCommand(AdminFlags.Fun)]
    sealed class AddHandCommand : IConsoleCommand
    {
        [Dependency] private readonly IEntityManager _entManager = default!;
        [Dependency] private readonly IPrototypeManager _protoManager = default!;

        private static int _handIdAccumulator;

        public string Command => "addhand";
        public string Description => "Adds a hand to your entity.";
        public string Help => $"Usage: {Command} <entityUid>";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var player = shell.Player;

            EntityUid entity;

            switch (args.Length)
            {
                case 0:
                    if (player == null)
                    {
                        shell.WriteLine("Only a player can run this command without arguments.");
                        return;
                    }

                    if (player.AttachedEntity == null)
                    {
                        shell.WriteLine("You don't have an entity to add a hand to.");
                        return;
                    }

                    entity = player.AttachedEntity.Value;
                    break;
                case 1:
                    {
                        if (NetEntity.TryParse(args[0], out var uidNet) && _entManager.TryGetEntity(uidNet, out var uid))
                        {
                            if (!_entManager.EntityExists(uid))
                            {
                                shell.WriteLine($"No entity found with uid {uid}");
                                return;
                            }

                            entity = uid.Value;
                        }
                        else
                        {
                            if (player == null)
                            {
                                shell.WriteLine("You must specify an entity to add a hand to when using this command from the server terminal.");
                                return;
                            }

                            if (player.AttachedEntity == null)
                            {
                                shell.WriteLine("You don't have an entity to add a hand to.");
                                return;
                            }

                            entity = player.AttachedEntity.Value;
                        }

                        break;
                    }
                default:
                    shell.WriteLine(Help);
                    return;
            }

            _entManager.System<HandsSystem>().AddHand(entity, $"cmd-{_handIdAccumulator++}", HandLocation.Middle);

            shell.WriteLine($"Added hand to entity {_entManager.GetComponent<MetaDataComponent>(entity).EntityName}");
        }
    }
}
