// SPDX-FileCopyrightText: 2025 Winkarst
// SPDX-FileCopyrightText: 2024 lzk
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Pancake
// SPDX-License-Identifier: MIT

using Content.Server.Speech.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Speech.Components;

/// <summary>
/// Replaces full sentences or words within sentences with new strings.
/// </summary>
[RegisterComponent]
public sealed partial class ReplacementAccentComponent : Component
{
    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<ReplacementAccentPrototype>), required: true)]
    public string Accent = default!;

}
