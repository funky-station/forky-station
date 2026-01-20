// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Callmore
// SPDX-FileCopyrightText: 2025 Kyle Tyo
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 RiceMar1244
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Allows the entity to be fired from a gun.
/// </summary>
[RegisterComponent, Virtual]
public partial class AmmoComponent : Component, IShootable
{
    // Muzzle flash stored on ammo because if we swap a gun to whatever we may want to override it.

    [DataField]
    public EntProtoId? MuzzleFlash = "MuzzleFlashEffect";
}

/// <summary>
/// Spawns another prototype to be shot instead of itself.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)]
public sealed partial class CartridgeAmmoComponent : AmmoComponent
{
    /// <summary>
    /// Prototype of the ammo to be shot.
    /// </summary>
    [DataField("proto", required: true)]
    public EntProtoId Prototype;

    /// <summary>
    /// Is this cartridge spent?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Spent;

    /// <summary>
    /// Is this cartridge automatically marked as trash once spent?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool MarkSpentAsTrash = true;

    /// <summary>
    /// Caseless ammunition.
    /// </summary>
    [DataField]
    public bool DeleteOnSpawn;

    /// <summary>
    /// Sound the case makes when it leaves the weapon.
    /// </summary>
    [DataField("soundEject")]
    public SoundSpecifier? EjectSound = new SoundCollectionSpecifier("CasingEject");
}
