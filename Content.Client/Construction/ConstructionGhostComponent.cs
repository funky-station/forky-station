// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2018 PJB3005
// SPDX-License-Identifier: MIT

using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Construction
{
    [RegisterComponent]
    public sealed partial class ConstructionGhostComponent : Component
    {
        public int GhostId { get; set; }
        [ViewVariables] public ConstructionPrototype? Prototype { get; set; }
    }
}
