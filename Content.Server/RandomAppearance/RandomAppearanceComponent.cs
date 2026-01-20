// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Server.RandomAppearance;

[RegisterComponent]
[Access(typeof(RandomAppearanceSystem))]
public sealed partial class RandomAppearanceComponent : Component
{
    [DataField("spriteStates")]
    public string[] SpriteStates = { "0", "1", "2", "3", "4" };

    /// <summary>
    ///     What appearance enum key should be set to the random sprite state?
    /// </summary>
    [DataField(required: true, customTypeSerializer: typeof(EnumSerializer))]
    public Enum? EnumKey;
}
