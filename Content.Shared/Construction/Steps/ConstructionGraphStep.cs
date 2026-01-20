// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Examine;

namespace Content.Shared.Construction.Steps
{
    [Serializable]
    [ImplicitDataDefinitionForInheritors]
    public abstract partial class ConstructionGraphStep
    {
        [DataField("completed", serverOnly: true)] private IGraphAction[] _completed = Array.Empty<IGraphAction>();

        [DataField("doAfter")] public float DoAfter { get; private set; }

        public IReadOnlyList<IGraphAction> Completed => _completed;

        public abstract void DoExamine(ExaminedEvent examinedEvent);
        public abstract ConstructionGuideEntry GenerateGuideEntry();
    }
}
