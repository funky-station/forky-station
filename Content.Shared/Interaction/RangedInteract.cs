// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;
using Robust.Shared.Map;

namespace Content.Shared.Interaction
{
    /// <summary>
    ///     Raised when an entity is interacted with that is out of the user entity's range of direct use.
    /// </summary>
    [PublicAPI]
    public sealed class RangedInteractEvent : HandledEntityEventArgs
    {
        /// <summary>
        ///     Entity that triggered the interaction.
        /// </summary>
        public EntityUid UserUid { get; }

        /// <summary>
        ///     Entity that the user used to interact.
        /// </summary>
        public EntityUid UsedUid { get; }

        /// <summary>
        ///     Entity that was interacted on.
        /// </summary>
        public EntityUid TargetUid { get; }

        /// <summary>
        ///     Location that the user clicked outside of their interaction range.
        /// </summary>
        public EntityCoordinates ClickLocation { get; }

        public RangedInteractEvent(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation)
        {
            UserUid = user;
            UsedUid = used;
            TargetUid = target;
            ClickLocation = clickLocation;
        }
    }
}
