// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2024 Killerqu00
// SPDX-License-Identifier: MIT

using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Zombies;

[RegisterComponent, NetworkedComponent]
public sealed partial class InitialInfectedComponent : Component
{
    [DataField]
    public ProtoId<FactionIconPrototype> StatusIcon = "InitialInfectedFaction";
}
