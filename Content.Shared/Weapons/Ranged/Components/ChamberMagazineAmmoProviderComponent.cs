// SPDX-FileCopyrightText: 2022, 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TaralGit <76408146+TaralGit@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 BramvanZijp <56019239+BramvanZijp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 mq <113324899+mqole@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;

namespace Content.Shared.Weapons.Ranged.Components;

/// <summary>
/// Chamber + mags in one package. If you need just magazine then use <see cref="MagazineAmmoProviderComponent"/>
/// </summary>
[RegisterComponent, AutoGenerateComponentState]
[Access(typeof(SharedGunSystem))]
public sealed partial class ChamberMagazineAmmoProviderComponent : MagazineAmmoProviderComponent
{
    /// <summary>
    /// If the gun has a bolt and whether that bolt is closed. Firing is impossible
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool? BoltClosed = false;

    /// <summary>
    /// Does the gun automatically open and close the bolt upon shooting.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool AutoCycle = true;

    /// <summary>
    /// Can the gun be racked, which opens and then instantly closes the bolt to cycle a round.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool CanRack = true;

    [DataField("soundBoltClosed"), AutoNetworkedField]
    public SoundSpecifier? BoltClosedSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Bolt/rifle_bolt_closed.ogg");

    [DataField("soundBoltOpened"), AutoNetworkedField]
    public SoundSpecifier? BoltOpenedSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Bolt/rifle_bolt_open.ogg");

    [DataField("soundRack"), AutoNetworkedField]
    public SoundSpecifier? RackSound = new SoundPathSpecifier("/Audio/Weapons/Guns/Cock/ltrifle_cock.ogg");
}
