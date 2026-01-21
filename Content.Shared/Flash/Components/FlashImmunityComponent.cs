// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 B_Kirill <153602297+B-Kirill@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Flash.Components;

/// <summary>
/// Makes the entity immune to being flashed.
/// When given to clothes in the "head", "eyes" or "mask" slot it protects the wearer.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedFlashSystem))]
public sealed partial class FlashImmunityComponent : Component
{
    /// <summary>
    /// Is this component currently enabled?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    /// <summary>
    /// Should the flash protection be shown when examining the entity?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool ShowInExamine = true;
}
