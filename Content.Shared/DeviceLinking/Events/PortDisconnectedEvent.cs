// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

namespace Content.Shared.DeviceLinking.Events
{
    public sealed class PortDisconnectedEvent : EntityEventArgs
    {
        public readonly string Port;

        public PortDisconnectedEvent(string port)
        {
            Port = port;
        }
    }
}
