// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-License-Identifier: MIT

namespace Content.Shared.Xenoarchaeology.XenoArtifacts;

[RegisterComponent]
public sealed partial class RandomArtifactSpriteComponent : Component
{
    [DataField("minSprite")]
    public int MinSprite = 1;

    [DataField("maxSprite")]
    public int MaxSprite = 14;

    [DataField("activationTime")]
    public double ActivationTime = 0.4;

    public TimeSpan? ActivationStart;
}
