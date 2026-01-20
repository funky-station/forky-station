// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

namespace Content.Server.Holiday.Interfaces
{
    public interface IHolidayGreet
    {
        string Greet(HolidayPrototype holiday);
    }
}
