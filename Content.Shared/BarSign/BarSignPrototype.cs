// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.BarSign;

[Prototype]
public sealed partial class BarSignPrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public SpriteSpecifier Icon { get; private set; } = default!;

    [DataField]
    public LocId Name { get; private set; } = "barsign-component-name";

    [DataField]
    public LocId Description { get; private set; }

    [DataField]
    public bool Hidden { get; private set; }
}
