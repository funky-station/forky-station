// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Special component to allow an entity to navigate kudzu without slowdown.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class IgnoreKudzuComponent : Component
{
}
