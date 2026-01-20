// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Server.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Events;
using JetBrains.Annotations;

namespace Content.Server.DeviceNetwork.Systems
{
    [UsedImplicitly]
    public sealed class WiredNetworkSystem : EntitySystem
    {
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<WiredNetworkComponent, BeforePacketSentEvent>(OnBeforePacketSent);
        }

        /// <summary>
        /// Checks if both devices are on the same grid
        /// </summary>
        private void OnBeforePacketSent(EntityUid uid, WiredNetworkComponent component, BeforePacketSentEvent args)
        {
            if (Transform(uid).GridUid != args.SenderTransform.GridUid)
            {
                args.Cancel();
            }
        }

        //Things to do in a future PR:
        //Abstract out the connection between the apcExtensionCable and the apcPowerReceiver
        //Traverse the power cables using path traversal
        //Cache an optimized representation of the traversed path (Probably just cache Devices)
    }
}
