// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Peptide90
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Construction;
using Content.Shared.Examine;
using Content.Shared.Lock;
using JetBrains.Annotations;

namespace Content.Server.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class Locked : IGraphCondition
    {
        [DataField("locked")]
        public bool IsLocked { get; private set; } = true;

        public bool Condition(EntityUid uid, IEntityManager entityManager)
        {
            if (!entityManager.TryGetComponent(uid, out LockComponent? lockcomp))
                return true;

            return lockcomp.Locked == IsLocked;
        }

        public bool DoExamine(ExaminedEvent args)
        {
            var entMan = IoCManager.Resolve<IEntityManager>();
            var entity = args.Examined;

            if (!entMan.TryGetComponent(entity, out LockComponent? lockcomp))
                return true;

            switch (IsLocked)
            {
                case true when !lockcomp.Locked:
                    args.PushMarkup(Loc.GetString("construction-examine-condition-lock"));
                    return true;
                case false when lockcomp.Locked:
                    args.PushMarkup(Loc.GetString("construction-examine-condition-unlock"));
                    return true;
            }

            return false;
        }

        public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
        {
            yield return new ConstructionGuideEntry()
            {
                Localization = IsLocked
                    ? "construction-step-condition-wire-panel-lock"
                    : "construction-step-condition-wire-panel-unlock"
            };
        }
    }
}
