// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Francesco
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Medical.Cryogenics;

/// <summary>
/// Tracking component for an enabled cryo pod (which periodically tries to inject chemicals in the occupant, if one exists)
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveCryoPodComponent : Component;
