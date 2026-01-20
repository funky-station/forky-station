// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Peptide90
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Git-Nivrak
// SPDX-License-Identifier: MIT

using Content.Shared.Construction;
using Content.Shared.Examine;
using Content.Shared.Storage.Components;
using Content.Shared.Tools.Systems;
using JetBrains.Annotations;

namespace Content.Server.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public sealed partial class StorageWelded : IGraphCondition
    {
        [DataField("welded")]
        public bool Welded { get; private set; } = true;

        public bool Condition(EntityUid uid, IEntityManager entityManager)
        {
            return entityManager.System<WeldableSystem>().IsWelded(uid) == Welded;
        }

        public bool DoExamine(ExaminedEvent args)
        {
            var entMan = IoCManager.Resolve<IEntityManager>();
            var entity = args.Examined;

            if (!entMan.HasComponent<EntityStorageComponent>(entity))
                return false;

            var metaData = entMan.GetComponent<MetaDataComponent>(entity);

            if (entMan.System<WeldableSystem>().IsWelded(entity) != Welded)
            {
                if (Welded)
                    args.PushMarkup(Loc.GetString("construction-examine-condition-door-weld", ("entityName", metaData.EntityName)) + "\n");
                else
                    args.PushMarkup(Loc.GetString("construction-examine-condition-door-unweld", ("entityName", metaData.EntityName)) + "\n");
                return true;
            }

            return false;
        }

        public IEnumerable<ConstructionGuideEntry> GenerateGuideEntry()
        {
            yield return new ConstructionGuideEntry()
            {
                Localization = Welded
                    ? "construction-guide-condition-door-weld"
                    : "construction-guide-condition-door-unweld",
            };
        }
    }
}
