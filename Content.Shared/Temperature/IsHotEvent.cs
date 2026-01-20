// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Shared.Temperature
{
    /// <summary>
    ///     Directed event raised on entities to query whether they're "hot" or not.
    ///     For example, a lit welder or matchstick would be hot, etc.
    /// </summary>
    public sealed class IsHotEvent : EntityEventArgs
    {
        public bool IsHot { get; set; } = false;
    }
}
