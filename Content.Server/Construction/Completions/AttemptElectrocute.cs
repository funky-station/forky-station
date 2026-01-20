// SPDX-FileCopyrightText: 2024 Fildrance
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Server.Electrocution;
using Content.Shared.Electrocution;
using Content.Shared.Construction;

namespace Content.Server.Construction.Completions;

[DataDefinition]
public sealed partial class AttemptElectrocute : IGraphAction
{
    public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
    {
        if (userUid == null)
            return;

        if (!entityManager.TryGetComponent<ElectrifiedComponent>(uid, out var electrified))
            return;

        var currentValue = electrified.Enabled;
        electrified.Enabled = true;

        entityManager.EntitySysManager.GetEntitySystem<ElectrocutionSystem>().TryDoElectrifiedAct(uid, userUid.Value, electrified: electrified);

        electrified.Enabled = currentValue;
    }
}
