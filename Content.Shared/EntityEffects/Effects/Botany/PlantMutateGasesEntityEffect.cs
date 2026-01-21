// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.EntityEffects.Effects.Botany;

/// <summary>
/// See serverside system.
/// </summary>
public sealed partial class PlantMutateConsumeGases : EntityEffectBase<PlantMutateConsumeGases>
{
    [DataField]
    public float MinValue = 0.01f;

    [DataField]
    public float MaxValue = 0.5f;
}

public sealed partial class PlantMutateExudeGases : EntityEffectBase<PlantMutateExudeGases>
{
    [DataField]
    public float MinValue = 0.01f;

    [DataField]
    public float MaxValue = 0.5f;
}
