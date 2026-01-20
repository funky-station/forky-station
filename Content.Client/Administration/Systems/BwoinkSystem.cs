// SPDX-FileCopyrightText: 2025 Winkarst
// SPDX-FileCopyrightText: 2024 dffdff2423
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Jezithyr
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Visne
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2022 20kdc
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Paul
// SPDX-FileCopyrightText: 2022 E F R
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

#nullable enable
using Content.Shared.Administration;
using JetBrains.Annotations;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client.Administration.Systems
{
    [UsedImplicitly]
    public sealed class BwoinkSystem : SharedBwoinkSystem
    {
        [Dependency] private readonly IGameTiming _timing = default!;

        public event EventHandler<BwoinkTextMessage>? OnBwoinkTextMessageRecieved;
        private (TimeSpan Timestamp, bool Typing) _lastTypingUpdateSent;

        protected override void OnBwoinkTextMessage(BwoinkTextMessage message, EntitySessionEventArgs eventArgs)
        {
            OnBwoinkTextMessageRecieved?.Invoke(this, message);
        }

        public void Send(NetUserId channelId, string text, bool playSound, bool adminOnly)
        {
            // Reuse the channel ID as the 'true sender'.
            // Server will ignore this and if someone makes it not ignore this (which is bad, allows impersonation!!!), that will help.
            RaiseNetworkEvent(new BwoinkTextMessage(channelId, channelId, text, playSound: playSound, adminOnly: adminOnly));
            SendInputTextUpdated(channelId, false);
        }

        public void SendInputTextUpdated(NetUserId channel, bool typing)
        {
            if (_lastTypingUpdateSent.Typing == typing &&
                _lastTypingUpdateSent.Timestamp + TimeSpan.FromSeconds(1) > _timing.RealTime)
            {
                return;
            }

            _lastTypingUpdateSent = (_timing.RealTime, typing);
            RaiseNetworkEvent(new BwoinkClientTypingUpdated(channel, typing));
        }
    }
}
