// SPDX-FileCopyrightText: 2025 BarryNorfolk
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Content.Server.ParticleAccelerator.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Machines.Components;
using Content.Shared.ParticleAccelerator.Components;

namespace Content.Server.ParticleAccelerator.EntitySystems;

public sealed partial class ParticleAcceleratorSystem
{
    private void InitializePowerBoxSystem()
    {
        SubscribeLocalEvent<ParticleAcceleratorPowerBoxComponent, PowerConsumerReceivedChanged>(PowerBoxReceivedChanged);
    }

    private void PowerBoxReceivedChanged(EntityUid uid, ParticleAcceleratorPowerBoxComponent component, ref PowerConsumerReceivedChanged args)
    {
        if (!TryComp<MultipartMachinePartComponent>(uid, out var part))
            return;
        if (!TryComp<ParticleAcceleratorControlBoxComponent>(part.Master, out var controller))
            return;

        var master = part.Master!.Value;
        if (controller.Enabled && args.ReceivedPower >= args.DrawRate * ParticleAcceleratorControlBoxComponent.RequiredPowerRatio)
            PowerOn(master, comp: controller);
        else
            PowerOff(master, comp: controller);

        UpdateUI(master, controller);
    }
}
