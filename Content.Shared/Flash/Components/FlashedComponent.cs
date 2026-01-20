// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Flash.Components;

/// <summary>
/// Exists for use as a status effect. Adds a shader to the client that obstructs vision.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FlashedComponent : Component;
