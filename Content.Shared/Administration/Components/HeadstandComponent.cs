// SPDX-FileCopyrightText: 2025 Pok
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Administration.Components;

/// <summary>
/// Flips the target's sprite on its head, so they do a headstand.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HeadstandComponent : Component;
