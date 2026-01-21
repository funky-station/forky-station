// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Server.Implants.Components;

/// <summary>
/// Implants an entity automatically on MapInit.
/// </summary>
[RegisterComponent]
public sealed partial class AutoImplantComponent : Component
{
    /// <summary>
    /// List of implants to inject.
    /// </summary>
    [DataField(required: true)]
    public List<EntProtoId> Implants = new();
}
