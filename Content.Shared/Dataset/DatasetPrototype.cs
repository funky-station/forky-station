// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Dataset
{
    [Prototype]
    public sealed partial class DatasetPrototype : IPrototype
    {
        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;

        [DataField("values")] public IReadOnlyList<string> Values { get; private set; } = new List<string>();
    }
}
