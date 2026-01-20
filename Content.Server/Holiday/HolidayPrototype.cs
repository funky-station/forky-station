// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Holiday.Greet;
using Content.Server.Holiday.Interfaces;
using Content.Server.Holiday.ShouldCelebrate;
using Robust.Shared.Prototypes;

namespace Content.Server.Holiday
{
    [Prototype]
    public sealed partial class HolidayPrototype : IPrototype
    {
        [DataField("name")] public string Name { get; private set; } = string.Empty;

        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;

        [DataField("beginDay")]
        public byte BeginDay { get; set; } = 1;

        [DataField("beginMonth")]
        public Month BeginMonth { get; set; } = Month.Invalid;

        /// <summary>
        ///     Day this holiday will end. Zero means it lasts a single day.
        /// </summary>
        [DataField("endDay")]
        public byte EndDay { get; set; }

        /// <summary>
        ///     Month this holiday will end in. Invalid means it lasts a single month.
        /// </summary>
        [DataField("endMonth")]
        public Month EndMonth { get; set; } = Month.Invalid;

        [DataField("shouldCelebrate")]
        private IHolidayShouldCelebrate _shouldCelebrate = new DefaultHolidayShouldCelebrate();

        [DataField("greet")]
        private IHolidayGreet _greet = new DefaultHolidayGreet();

        [DataField("celebrate")]
        private IHolidayCelebrate? _celebrate = null;

        public bool ShouldCelebrate(DateTime date)
        {
            return _shouldCelebrate.ShouldCelebrate(date, this);
        }

        public string Greet()
        {
            return _greet.Greet(this);
        }

        /// <summary>
        ///     Called before the round starts to set up any festive shenanigans.
        /// </summary>
        public void Celebrate()
        {
            _celebrate?.Celebrate(this);
        }
    }
}
