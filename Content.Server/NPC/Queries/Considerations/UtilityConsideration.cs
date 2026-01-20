// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Server.NPC.Queries.Curves;
using JetBrains.Annotations;

namespace Content.Server.NPC.Queries.Considerations;

[ImplicitDataDefinitionForInheritors, MeansImplicitUse]
public abstract partial class UtilityConsideration
{
    [DataField("curve", required: true)]
    public IUtilityCurve Curve = default!;
}
