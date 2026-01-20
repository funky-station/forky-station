// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-License-Identifier: MIT

using Content.Shared.Implants;
using Content.Shared.Roles;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.Jobs;

/// <summary>
/// Adds implants on spawn to the entity
/// </summary>
[UsedImplicitly]
public sealed partial class AddImplantSpecial : JobSpecial
{
    [DataField]
    public HashSet<EntProtoId> Implants { get; private set; } = new();

    public override void AfterEquip(EntityUid mob)
    {
        var entMan = IoCManager.Resolve<IEntityManager>();
        var implantSystem = entMan.System<SharedSubdermalImplantSystem>();
        implantSystem.AddImplants(mob, Implants);
    }
}
