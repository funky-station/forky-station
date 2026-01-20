// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;
using Robust.Shared.GameStates;

namespace Content.Shared.Parallax;

/// <summary>
/// Handles per-map parallax
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class ParallaxComponent : Component
{
    // I wish I could use a typeserializer here but parallax is extremely client-dependent.
    [DataField, AutoNetworkedField]
    public string Parallax = "Default";
}
