// SPDX-FileCopyrightText: 2025 Ertanic
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Examine;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Steps
{
    public abstract partial class ArbitraryInsertConstructionGraphStep : EntityInsertConstructionGraphStep
    {
        [DataField] public LocId Name { get; private set; } = string.Empty;

        [DataField] public SpriteSpecifier? Icon { get; private set; }

        public override void DoExamine(ExaminedEvent examinedEvent)
        {
            if (string.IsNullOrEmpty(Name))
                return;

            var stepName = Loc.GetString(Name);
            examinedEvent.PushMarkup(Loc.GetString("construction-insert-arbitrary-entity", ("stepName", stepName)));
        }

        public override ConstructionGuideEntry GenerateGuideEntry()
        {
            var stepName = Loc.GetString(Name);
            return new ConstructionGuideEntry
            {
                Localization = "construction-presenter-arbitrary-step",
                Arguments = new (string, object)[]{("name", stepName)},
                Icon = Icon,
            };
        }
    }
}
