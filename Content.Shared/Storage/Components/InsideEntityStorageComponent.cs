// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Storage.Components;

/// <summary>
/// Added to entities contained within entity storage, for directed event purposes.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class InsideEntityStorageComponent : Component
{
    /// <summary>
    /// The entity storage this entity is inside.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid Storage;
}
