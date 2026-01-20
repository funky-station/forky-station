// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Holiday.Interfaces;

namespace Content.Server.Holiday.Greet
{
    [DataDefinition]
    public sealed partial class DefaultHolidayGreet : IHolidayGreet
    {
        public string Greet(HolidayPrototype holiday)
        {
            var holidayName = Loc.GetString(holiday.Name);
            return Loc.GetString("holiday-greet", ("holidayName", holidayName));
        }
    }
}
