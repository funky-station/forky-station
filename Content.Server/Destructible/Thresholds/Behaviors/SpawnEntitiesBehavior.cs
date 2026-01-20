// SPDX-FileCopyrightText: 2025 āda
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 Whisper
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2024 Łukasz Mędrek
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 faint
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 AJCM-git
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Server.Forensics;
using Content.Server.Stack;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.Prototypes;
using Content.Shared.Stacks;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Destructible.Thresholds.Behaviors
{
    [Serializable]
    [DataDefinition]
    public sealed partial class SpawnEntitiesBehavior : IThresholdBehavior
    {
        /// <summary>
        ///     Entities spawned on reaching this threshold, from a min to a max.
        /// </summary>
        [DataField]
        public Dictionary<EntProtoId, MinMax> Spawn = new();

        [DataField("offset")]
        public float Offset { get; set; } = 0.5f;

        [DataField("transferForensics")]
        public bool DoTransferForensics;

        [DataField]
        public bool SpawnInContainer;

        public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
        {
            var tSys = system.EntityManager.System<TransformSystem>();
            var position = tSys.GetMapCoordinates(owner);

            var getRandomVector = () => new Vector2(system.Random.NextFloat(-Offset, Offset), system.Random.NextFloat(-Offset, Offset));

            var executions = 1;
            if (system.EntityManager.TryGetComponent<StackComponent>(owner, out var stack))
            {
                executions = stack.Count;
            }

            foreach (var (entityId, minMax) in Spawn)
            {
                for (var execution = 0; execution < executions; execution++)
                {
                    var count = minMax.Min >= minMax.Max
                        ? minMax.Min
                        : system.Random.Next(minMax.Min, minMax.Max + 1);

                    if (count == 0)
                        continue;

                    if (EntityPrototypeHelpers.HasComponent<StackComponent>(entityId, system.PrototypeManager, system.EntityManager.ComponentFactory))
                    {
                        var spawned = SpawnInContainer
                            ? system.EntityManager.SpawnNextToOrDrop(entityId, owner)
                            : system.EntityManager.SpawnEntity(entityId, position.Offset(getRandomVector()));
                        system.StackSystem.SetCount((spawned, null), count);

                        TransferForensics(spawned, system, owner);
                    }
                    else
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var spawned = SpawnInContainer
                                ? system.EntityManager.SpawnNextToOrDrop(entityId, owner)
                                : system.EntityManager.SpawnEntity(entityId, position.Offset(getRandomVector()));

                            TransferForensics(spawned, system, owner);
                        }
                    }
                }
            }
        }

        public void TransferForensics(EntityUid spawned, DestructibleSystem system, EntityUid owner)
        {
            if (!DoTransferForensics ||
                !system.EntityManager.TryGetComponent<ForensicsComponent>(owner, out var forensicsComponent))
                return;

            var comp = system.EntityManager.EnsureComponent<ForensicsComponent>(spawned);
            comp.DNAs = forensicsComponent.DNAs;

            if (!system.Random.Prob(0.4f))
                return;
            comp.Fingerprints = forensicsComponent.Fingerprints;
            comp.Fibers = forensicsComponent.Fibers;
        }
    }
}
