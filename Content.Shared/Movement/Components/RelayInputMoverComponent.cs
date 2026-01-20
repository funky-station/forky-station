// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Movement.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Raises the engine movement inputs for a particular entity onto the designated entity
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(SharedMoverController))]
public sealed partial class RelayInputMoverComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid RelayEntity;
}
