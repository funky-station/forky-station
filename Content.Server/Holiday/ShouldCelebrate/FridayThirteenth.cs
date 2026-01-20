// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Holiday.Interfaces;
using JetBrains.Annotations;

namespace Content.Server.Holiday.ShouldCelebrate
{
    /// <summary>
    ///     For Friday the 13th. Spooky!
    /// </summary>
    [UsedImplicitly]
    public sealed partial class FridayThirteenth : IHolidayShouldCelebrate
    {
        public bool ShouldCelebrate(DateTime date, HolidayPrototype holiday)
        {
            return date.Day == 13 && date.DayOfWeek == DayOfWeek.Friday;
        }
    }
}
