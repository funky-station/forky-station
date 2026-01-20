// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2024 Aexxie
// SPDX-FileCopyrightText: 2024 Emisse
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 GraniteSidewalk
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Remie Richards
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2020 ColdAutumnRain
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 R. Neuser
// SPDX-FileCopyrightText: 2020 Hugal31
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Flash.Components;

/// <summary>
/// Allows this entity to flash someone by using it or melee attacking with it.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedFlashSystem))]
public sealed partial class FlashComponent : Component
{
    /// <summary>
    /// Flash the area around the entity when used in hand?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool FlashOnUse = true;

    /// <summary>
    /// Flash the target when melee attacking them?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool FlashOnMelee = true;

    /// <summary>
    /// Time the Flash will be visually flashing after use.
    /// For the actual interaction delay use UseDelayComponent.
    /// These two times should be the same.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan FlashingTime = TimeSpan.FromSeconds(4);

    /// <summary>
    /// For how long the target will lose vision when melee attacked with the flash.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan MeleeDuration = TimeSpan.FromSeconds(5);

    /// <summary>
    /// For how long the target will lose vision when used in hand.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan AoeFlashDuration = TimeSpan.FromSeconds(2);

    /// <summary>
    /// How long a target is stunned when a melee flash is used.
    /// If null, melee flashes will not stun at all.
    /// </summary>
    [DataField, AutoNetworkedField]
    public TimeSpan? MeleeStunDuration = TimeSpan.FromSeconds(1.5);

    /// <summary>
    /// Range of the flash when using it.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Range = 7f;

    /// <summary>
    /// Movement speed multiplier for slowing down the target while they are flashed.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float SlowTo = 0.5f;

    /// <summary>
    /// The sound to play when flashing.
    /// </summary>

    [DataField, AutoNetworkedField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Weapons/flash.ogg")
    {
        Params = AudioParams.Default.WithVolume(1f).WithMaxDistance(3f)
    };

    /// <summary>
    /// The probability of sucessfully flashing someone.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float Probability = 1f;
}
