// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Construction;
using JetBrains.Annotations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Random;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class PlaySound : IGraphAction
    {
        [DataField("sound", required: true)] public SoundSpecifier Sound { get; private set; } = default!;

        [DataField("AudioParams")]
        public AudioParams AudioParams = AudioParams.Default;

        [ViewVariables(VVAccess.ReadWrite)]
        [DataField("variation")]
        public float Variation = 0.125f;

        public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
        {
            var scale = (float) IoCManager.Resolve<IRobustRandom>().NextGaussian(1, Variation);
            if (entityManager.TryGetComponent<TransformComponent>(uid, out var xform))
                entityManager.EntitySysManager.GetEntitySystem<SharedAudioSystem>()
                .PlayPvs(Sound, xform.Coordinates, AudioParams.WithPitchScale(scale));
        }
    }
}
