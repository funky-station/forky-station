// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Holiday.Interfaces;
using JetBrains.Annotations;

namespace Content.Server.Holiday.Greet
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class Custom : IHolidayGreet
    {
        [DataField("text")] private string _greet = string.Empty;

        public string Greet(HolidayPrototype holiday)
        {
            return Loc.GetString(_greet);
        }
    }
}
