// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events
{
    /// <summary>
    /// An event ot tell the server that we have stopped playing this tabletop game.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class TabletopStopPlayingEvent : EntityEventArgs
    {
        /// <summary>
        /// The entity UID of the table associated with this tabletop game.
        /// </summary>
        public NetEntity TableUid;

        public TabletopStopPlayingEvent(NetEntity tableUid)
        {
            TableUid = tableUid;
        }
    }
}
