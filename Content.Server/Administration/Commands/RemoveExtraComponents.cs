// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
    [AdminCommand(AdminFlags.Mapping)]
    public sealed class RemoveExtraComponents : LocalizedEntityCommands
    {
        [Dependency] private readonly IComponentFactory _compFactory = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public override string Command => "removeextracomponents";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var id = args.Length == 0 ? null : string.Join(" ", args);

            EntityPrototype? prototype = null;
            var checkPrototype = !string.IsNullOrEmpty(id);

            if (checkPrototype && !_prototypeManager.TryIndex(id!, out prototype))
            {
                shell.WriteError(Loc.GetString($"cmd-removeextracomponents-invalid-prototype-id", ("id", $"{id}")));
                return;
            }

            var entities = 0;
            var components = 0;

            foreach (var entity in EntityManager.GetEntities())
            {
                var metaData = EntityManager.GetComponent<MetaDataComponent>(entity);
                if (checkPrototype && metaData.EntityPrototype != prototype || metaData.EntityPrototype == null)
                    continue;

                var modified = false;

                foreach (var component in EntityManager.GetComponents(entity))
                {
                    if (metaData.EntityPrototype.Components.ContainsKey(_compFactory.GetComponentName(component.GetType())))
                        continue;

                    EntityManager.RemoveComponent(entity, component);
                    components++;

                    modified = true;
                }

                if (modified)
                    entities++;
            }

            if (id != null)
            {
                shell.WriteLine(Loc.GetString($"cmd-removeextracomponents-success-with-id",
                    ("count", components),
                    ("entities", entities),
                    ("id", id)));
                return;
            }

            shell.WriteLine(Loc.GetString($"cmd-removeextracomponents-success",
                ("count", components),
                ("entities", entities)));
        }
    }
}
