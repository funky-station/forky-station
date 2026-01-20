// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2024 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Julian Giebel
// SPDX-License-Identifier: MIT

using Content.Server.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Events;
using JetBrains.Annotations;

namespace Content.Server.DeviceNetwork.Systems
{
    [UsedImplicitly]
    public sealed class WirelessNetworkSystem : EntitySystem
    {
        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<WirelessNetworkComponent, BeforePacketSentEvent>(OnBeforePacketSent);
        }

        /// <summary>
        /// Gets the position of both the sending and receiving entity and checks if the receiver is in range of the sender.
        /// </summary>
        private void OnBeforePacketSent(EntityUid uid, WirelessNetworkComponent component, BeforePacketSentEvent args)
        {
            var ownPosition = args.SenderPosition;
            var xform = Transform(uid);

            // not a wireless to wireless connection, just let it happen
            if (!TryComp<WirelessNetworkComponent>(args.Sender, out var sendingComponent))
                return;

            if (xform.MapID != args.SenderTransform.MapID
                || (ownPosition - _transformSystem.GetWorldPosition(xform)).Length() > sendingComponent.Range)
            {
                args.Cancel();
            }
        }
    }
}
