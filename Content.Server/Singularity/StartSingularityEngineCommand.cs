// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2025 BarryNorfolk
// SPDX-FileCopyrightText: 2022 TemporalOroboros
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 0x6273
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Machines.EntitySystems;
using Content.Server.ParticleAccelerator.Components;
using Content.Server.ParticleAccelerator.EntitySystems;
using Content.Server.Singularity.Components;
using Content.Server.Singularity.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Machines.Components;
using Content.Shared.Singularity.Components;
using Robust.Shared.Console;

namespace Content.Server.Singularity
{
    [AdminCommand(AdminFlags.Admin)]
    public sealed class StartSingularityEngineCommand : LocalizedEntityCommands
    {
        [Dependency] private readonly EmitterSystem _emitterSystem = default!;
        [Dependency] private readonly MultipartMachineSystem _multipartSystem = default!;
        [Dependency] private readonly ParticleAcceleratorSystem  _paSystem = default!;
        [Dependency] private readonly RadiationCollectorSystem _radCollectorSystem = default!;

        public override string Command => "startsingularityengine";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 0)
            {
                shell.WriteLine(Loc.GetString($"shell-need-exactly-zero-arguments"));
                return;
            }

            // Turn on emitters
            var emitterQuery = EntityManager.EntityQueryEnumerator<EmitterComponent>();
            while (emitterQuery.MoveNext(out var uid, out var emitterComponent))
            {
                //FIXME: This turns on ALL emitters, including APEs. It should only turn on the containment field emitters.
                _emitterSystem.SwitchOn(uid, emitterComponent);
            }

            // Turn on radiation collectors
            var radiationCollectorQuery = EntityManager.EntityQueryEnumerator<RadiationCollectorComponent>();
            while (radiationCollectorQuery.MoveNext(out var uid, out var radiationCollectorComponent))
            {
                _radCollectorSystem.SetCollectorEnabled(uid, enabled: true, user: null, radiationCollectorComponent);
            }

            // Setup PA
            var paQuery = EntityManager.EntityQueryEnumerator<ParticleAcceleratorControlBoxComponent>();
            while (paQuery.MoveNext(out var paId, out var paControl))
            {
                if (!EntityManager.TryGetComponent<MultipartMachineComponent>(paId, out var machine))
                    continue;

                if (!_multipartSystem.Rescan((paId, machine)))
                    continue;

                _paSystem.SetStrength(paId, ParticleAcceleratorPowerState.Level0, comp: paControl);
                _paSystem.SwitchOn(paId, comp: paControl);
            }

            shell.WriteLine(Loc.GetString($"shell-command-success"));
        }
    }
}
