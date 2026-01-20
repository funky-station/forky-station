// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Construction.Steps;

namespace Content.Shared.Construction
{
    [Serializable]
    [DataDefinition]
    public sealed partial class ConstructionGraphEdge
    {
        [DataField("steps")]
        private ConstructionGraphStep[] _steps = Array.Empty<ConstructionGraphStep>();

        [DataField("conditions", serverOnly: true)]
        private IGraphCondition[] _conditions = Array.Empty<IGraphCondition>();

        [DataField("completed", serverOnly: true)]
        private IGraphAction[] _completed = Array.Empty<IGraphAction>();

        [DataField("to", required:true)]
        public string Target { get; private set; } = string.Empty;

        [ViewVariables]
        public IReadOnlyList<IGraphCondition> Conditions => _conditions;

        [ViewVariables]
        public IReadOnlyList<IGraphAction> Completed => _completed;

        [ViewVariables]
        public IReadOnlyList<ConstructionGraphStep> Steps => _steps;
    }
}
