// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 Kara
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
