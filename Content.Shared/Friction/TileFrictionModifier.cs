// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Friction;

[RegisterComponent, NetworkedComponent]
[Access(typeof(TileFrictionController)), AutoGenerateComponentState]
public sealed partial class TileFrictionModifierComponent : Component
{
    /// <summary>
    ///     Multiply the tilefriction cvar by this to get the body's actual tilefriction.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("modifier"), AutoNetworkedField]
    public float Modifier;
}
