// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

namespace Content.Server.Access
{
    public sealed class AccessReaderChangeEvent : EntityEventArgs
    {
        public EntityUid Sender { get; }

        public bool Enabled { get; }

        public AccessReaderChangeEvent(EntityUid entity, bool enabled)
        {
            Sender = entity;
            Enabled = enabled;
        }
    }
}
