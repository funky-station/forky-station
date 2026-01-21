// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Connor Huffine <chuffine@gmail.com>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Clothing;

/// <summary>
/// Modifies speed when worn and activated.
/// Supports <see cref="ItemToggleComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(ClothingSpeedModifierSystem))]
public sealed partial class ClothingSpeedModifierComponent : Component
{
    [DataField]
    public float WalkModifier = 1.0f;

    [DataField]
    public float SprintModifier = 1.0f;

    /// <summary>
    /// Defines if the speed modifier requires <see cref="ItemToggleComponent"/> activation to apply.
    /// This will have no effect without an <see cref="ItemToggleComponent"/> on the entity.
    /// </summary>
    [DataField]
    public bool RequireActivated = true;

    /// <summary>
    /// An optional required standing state.
    /// Set to true if you need to be standing, false if you need to not be standing, null if you don't care.
    /// </summary>
    [DataField]
    public bool? Standing;
}

[Serializable, NetSerializable]
public sealed class ClothingSpeedModifierComponentState : ComponentState
{
    public float WalkModifier;
    public float SprintModifier;

    public ClothingSpeedModifierComponentState(float walkModifier, float sprintModifier)
    {
        WalkModifier = walkModifier;
        SprintModifier = sprintModifier;
    }
}
