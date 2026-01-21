// SPDX-FileCopyrightText: 2024 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Server.Tiles;

/// <summary>
/// Applies effects upon stepping onto a tile.
/// </summary>
[RegisterComponent, Access(typeof(TileEntityEffectSystem))]
public sealed partial class TileEntityEffectComponent : Component
{
    /// <summary>
    /// List of effects that should be applied.
    /// </summary>
    [DataField]
    public List<EntityEffect> Effects = default!;
}
