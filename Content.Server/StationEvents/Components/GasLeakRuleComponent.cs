// SPDX-FileCopyrightText: 2024 Ilya246
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Server.StationEvents.Events;
using Content.Shared.Atmos;
using Robust.Shared.Map;

namespace Content.Server.StationEvents.Components;

[RegisterComponent, Access(typeof(GasLeakRule))]
public sealed partial class GasLeakRuleComponent : Component
{
    public readonly Gas[] LeakableGases =
    {
        Gas.Ammonia,
        Gas.Plasma,
        Gas.Tritium,
        Gas.Frezon,
        Gas.WaterVapor, // the fog
        Gas.BZ, // Funky atmos - /tg/ gases
        Gas.Healium, // Funky atmos - /tg/ gases
        Gas.Nitrium, // Funky atmos - /tg/ gases
        Gas.Pluoxium, // Funky atmos - /tg/ gases
        Gas.HyperNoblium, // Funky atmos - /tg/ gases
        Gas.ProtoNitrate, // Funky atmos - /tg/ gases
        Gas.Halon, // Funky atmos - /tg/ gases
        Gas.Helium, // Funky atmos - /tg/ gases
        Gas.AntiNoblium, // Funky atmos - /tg/ gases
    };

    /// <summary>
    ///     Running cooldown of how much time until another leak.
    /// </summary>
    public float TimeUntilLeak;

    /// <summary>
    ///     How long between more gas being added to the tile.
    /// </summary>
    public float LeakCooldown = 1.0f;

    // Event variables
    public EntityUid TargetStation;
    public EntityUid TargetGrid;
    public Vector2i TargetTile;
    public EntityCoordinates TargetCoords;
    public bool FoundTile;
    public Gas LeakGas;
    public float MolesPerSecond;
    public readonly int MinimumMolesPerSecond = 80;

    /// <summary>
    ///     Don't want to make it too fast to give people time to flee.
    /// </summary>
    public int MaximumMolesPerSecond = 200;

    public int MinimumGas = 1000;
    public int MaximumGas = 4000;
    public float SparkChance = 0.05f;
}
