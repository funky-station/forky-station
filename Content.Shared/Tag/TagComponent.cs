// SPDX-FileCopyrightText: 2024 Tornado Tech
// SPDX-FileCopyrightText: 2023 PixelTK
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Tag;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(TagSystem))]
public sealed partial class TagComponent : Component
{
    [DataField, ViewVariables, AutoNetworkedField]
    public HashSet<ProtoId<TagPrototype>> Tags = new();
}
