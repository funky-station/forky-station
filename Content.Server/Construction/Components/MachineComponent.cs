// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Clyybber
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Content.Shared.Construction.Components;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server.Construction.Components;

[RegisterComponent]
public sealed partial class MachineComponent : Component
{
    [DataField]
    public EntProtoId<MachineBoardComponent>? Board { get; private set; }

    [ViewVariables]
    public Container BoardContainer = default!;
    [ViewVariables]
    public Container PartContainer = default!;
}
