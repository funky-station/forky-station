// SPDX-FileCopyrightText: 2024 Verm
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.CharacterAppearance.Components;

[RegisterComponent]
public sealed partial class RandomHumanoidAppearanceComponent : Component
{
    [DataField("randomizeName")] public bool RandomizeName = true;
    /// <summary>
    /// After randomizing, sets the hair style to this, if possible
    /// </summary>
    [DataField] public string? Hair = null;
}
