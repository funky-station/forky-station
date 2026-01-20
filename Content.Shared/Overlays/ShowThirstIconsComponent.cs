// SPDX-FileCopyrightText: 2024 Krunklehorn
// SPDX-FileCopyrightText: 2023 PrPleGoo
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Overlays;

/// <summary>
///     This component allows you to see the thirstiness of mobs.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ShowThirstIconsComponent : Component { }
