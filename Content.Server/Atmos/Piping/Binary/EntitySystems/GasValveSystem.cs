// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 faint
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-License-Identifier: MIT

using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Atmos.Piping.Binary.Systems;
using Content.Shared.Audio;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems;

public sealed class GasValveSystem : SharedGasValveSystem
{
    [Dependency] private readonly SharedAmbientSoundSystem _ambientSoundSystem = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;

    public override void Set(EntityUid uid, GasValveComponent component, bool value)
    {
        base.Set(uid, component, value);

        if (_nodeContainer.TryGetNodes(uid, component.InletName, component.OutletName, out PipeNode? inlet, out PipeNode? outlet))
        {
            if (component.Open)
            {
                inlet.AddAlwaysReachable(outlet);
                outlet.AddAlwaysReachable(inlet);
                _ambientSoundSystem.SetAmbience(uid, true);
            }
            else
            {
                inlet.RemoveAlwaysReachable(outlet);
                outlet.RemoveAlwaysReachable(inlet);
                _ambientSoundSystem.SetAmbience(uid, false);
            }
        }
    }
}
