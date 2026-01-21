// SPDX-FileCopyrightText: 2022, 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Menshin <Menshin@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Alert;
using Content.Shared.Inventory;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Clothing;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedMagbootsSystem))]
public sealed partial class MagbootsComponent : Component
{
    [DataField]
    public ProtoId<AlertPrototype> MagbootsAlert = "Magboots";

    /// <summary>
    /// If true, the user must be standing on a grid or planet map to experience the weightlessness-canceling effect
    /// </summary>
    [DataField]
    public bool RequiresGrid = true;
}
