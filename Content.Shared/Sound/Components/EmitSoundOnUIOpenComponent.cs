// SPDX-FileCopyrightText: 2022 Morb <14136326+Morb0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023-2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 MendaxxDev <153332064+MendaxxDev@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared.Sound.Components;

/// <summary>
/// Simple sound emitter that emits sound on AfterActivatableUIOpenEvent
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EmitSoundOnUIOpenComponent : BaseEmitSoundComponent
{
    /// <summary>
    /// Blacklist for making the sound not play if certain entities open the UI
    /// </summary>
    [DataField]
    public EntityWhitelist Blacklist = new();
}
