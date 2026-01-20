// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Francesco
// SPDX-License-Identifier: MIT

using Content.Server.Atmos;
using Content.Shared.Atmos;

namespace Content.Server.Medical.Components;

[RegisterComponent]
public sealed partial class CryoPodAirComponent : Component
{
    /// <summary>
    /// Local air buffer that will be mixed with the pipenet, if one exists, per tick.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("gasMixture")]
    public GasMixture Air { get; set; } = new GasMixture(1000f);
}
