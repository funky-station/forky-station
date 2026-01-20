// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Construction;
using Content.Shared.Examine;
using JetBrains.Annotations;
using Robust.Shared.Utility;

namespace Content.Server.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class EntityAnchored : IGraphCondition
    {
        [DataField("anchored")] public bool Anchored { get; private set; } = true;

        public bool Condition(EntityUid uid, IEntityManager entityManager)
        {
            var transform = entityManager.GetComponent<TransformComponent>(uid);
            return transform.Anchored && Anchored || !transform.Anchored && !Anchored;
        }

        public bool DoExamine(ExaminedEvent args)
        {
            var entity = args.Examined;

            var anchored = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(entity).Anchored;

            switch (Anchored)
            {
                case true when !anchored:
                    args.PushMarkup(Loc.GetString("construction-examine-condition-entity-anchored"));
                    return true;
                case false when anchored:
                    args.PushMarkup(Loc.GetString("construction-examine-condition-entity-unanchored"));
                    return true;
            }

            return false;
        }

        public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
        {
            yield return new ConstructionGuideEntry()
            {
                Localization = Anchored
                    ? "construction-step-condition-entity-anchored"
                    : "construction-step-condition-entity-unanchored",
                Icon = new SpriteSpecifier.Rsi(new ("Objects/Tools/wrench.rsi"), "icon"),
            };
        }
    }
}
