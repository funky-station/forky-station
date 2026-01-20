// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2024 Tornado Tech
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Tag;

/// <summary>
/// Prototype representing a tag in YAML.
/// Meant to only have an ID property, as that is the only thing that
/// gets saved in TagComponent.
/// </summary>
[Prototype("Tag")]
public sealed partial class TagPrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = string.Empty;
}
