// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson <karakkaraz@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Effects;

/// <summary>
/// Launches the owner of this component when triggered.
/// If TargetUser is true, this launches the entity that was collided with instead (because the "user" is the thing that's caused the collision, i.e. the other object).
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class LaunchOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// A linear impulse applied to the target, measured in kg * m / s
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Impulse = 10.0f;
}
