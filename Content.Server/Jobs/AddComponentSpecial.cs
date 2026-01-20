// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 Mervill
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 Moony
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Ephememory
// SPDX-FileCopyrightText: 2021 Wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server.Jobs;

public sealed partial class AddComponentSpecial : JobSpecial
{
    [DataField(required: true)]
    public ComponentRegistry Components { get; private set; } = new();

    /// <summary>
    /// If this is true then existing components will be removed and replaced with these ones.
    /// </summary>
    [DataField]
    public bool RemoveExisting = true;

    public override void AfterEquip(EntityUid mob)
    {
        var entMan = IoCManager.Resolve<IEntityManager>();
        entMan.AddComponents(mob, Components, removeExisting: RemoveExisting);
    }
}
