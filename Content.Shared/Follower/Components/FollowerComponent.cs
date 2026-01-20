// SPDX-FileCopyrightText: 2025 Perry Fraser
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Kara D
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Follower.Components;

[RegisterComponent]
[Access(typeof(FollowerSystem))]
[NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FollowerComponent : Component
{
    [AutoNetworkedField, DataField("following")]
    public EntityUid Following;
}
