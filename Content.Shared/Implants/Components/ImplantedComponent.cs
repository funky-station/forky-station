// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-License-Identifier: MIT

using Robust.Shared.Containers;
using Robust.Shared.GameStates;

namespace Content.Shared.Implants.Components;

/// <summary>
/// Added to an entity via the <see cref="SharedImplanterSystem"/> on implant
/// Used in instances where mob info needs to be passed to the implant such as MobState triggers
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ImplantedComponent : Component
{
    [ViewVariables(VVAccess.ReadOnly)]
    public Container ImplantContainer = default!;
}
