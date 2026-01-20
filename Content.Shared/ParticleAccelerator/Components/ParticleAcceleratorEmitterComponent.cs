// SPDX-FileCopyrightText: 2025 BarryNorfolk
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.ParticleAccelerator.Components;

[RegisterComponent]
public sealed partial class ParticleAcceleratorEmitterComponent : Component
{
    [DataField]
    public EntProtoId EmittedPrototype = "ParticlesProjectile";

    [DataField("emitterType")]
    [ViewVariables(VVAccess.ReadWrite)]
    public ParticleAcceleratorEmitterType Type = ParticleAcceleratorEmitterType.Fore;

    public override string ToString()
    {
        return base.ToString() + $" EmitterType:{Type}";
    }
}

public enum ParticleAcceleratorEmitterType
{
    Port,
    Fore,
    Starboard
}
