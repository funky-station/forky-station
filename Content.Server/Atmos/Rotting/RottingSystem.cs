// SPDX-FileCopyrightText: 2026 pathetic meowmeow
// SPDX-FileCopyrightText: 2026 ScarKy0
// SPDX-FileCopyrightText: 2025 Hannah Giovanna Dawson
// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 slarticodefast
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 Gotimanga
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2024 Kara
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Guillaume E
// SPDX-FileCopyrightText: 2024 Kot
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Body.Events;
using Content.Shared.Damage.Systems;
using Content.Shared.Gibbing;
using Content.Shared.Temperature.Components;
using Robust.Server.Containers;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Rotting;

public sealed class RottingSystem : SharedRottingSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly ContainerSystem _container = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RottingComponent, GibbedBeforeDeletionEvent>(OnGibbed);

        SubscribeLocalEvent<TemperatureComponent, IsRottingEvent>(OnTempIsRotting);
    }

    private void OnGibbed(EntityUid uid, RottingComponent component, GibbedBeforeDeletionEvent args)
    {
        if (!TryComp<PhysicsComponent>(uid, out var physics))
            return;

        if (!TryComp<PerishableComponent>(uid, out var perishable))
            return;

        var molsToDump = perishable.MolsPerSecondPerUnitMass * physics.FixturesMass * (float)component.TotalRotTime.TotalSeconds;
        var tileMix = _atmosphere.GetTileMixture(uid, excite: true);
        tileMix?.AdjustMoles(Gas.Ammonia, molsToDump);
    }

    private void OnTempIsRotting(EntityUid uid, TemperatureComponent component, ref IsRottingEvent args)
    {
        if (args.Handled)
            return;
        args.Handled = component.CurrentTemperature < Atmospherics.T0C + 0.85f;
    }

    /// <summary>
    /// Is anything speeding up the decay?
    /// e.g. buried in a grave
    /// TODO: hot temperatures increase rot?
    /// </summary>
    /// <returns></returns>
    private float GetRotRate(EntityUid uid)
    {
        if (_container.TryGetContainingContainer((uid, null, null), out var container) &&
            TryComp<ProRottingContainerComponent>(container.Owner, out var rotContainer))
        {
            return rotContainer.DecayModifier;
        }

        return 1f;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var perishQuery = EntityQueryEnumerator<PerishableComponent>();
        while (perishQuery.MoveNext(out var uid, out var perishable))
        {
            if (_timing.CurTime < perishable.RotNextUpdate)
                continue;

            perishable.RotNextUpdate += perishable.PerishUpdateRate;

            var stage = PerishStage((uid, perishable), MaxStages);
            if (stage != perishable.Stage)
            {
                perishable.Stage = stage;
                DirtyField(uid, perishable, nameof(PerishableComponent.Stage));
            }

            if (IsRotten(uid) || !IsRotProgressing(uid, perishable))
                continue;

            perishable.RotAccumulator += perishable.PerishUpdateRate * GetRotRate(uid);
            DirtyField(uid, perishable, nameof(PerishableComponent.RotAccumulator));
            if (perishable.RotAccumulator >= perishable.RotAfter)
            {
                var rot = AddComp<RottingComponent>(uid);
                rot.NextRotUpdate = _timing.CurTime + rot.RotUpdateRate;
            }
        }

        var rotQuery = EntityQueryEnumerator<RottingComponent, PerishableComponent, TransformComponent>();
        while (rotQuery.MoveNext(out var uid, out var rotting, out var perishable, out var xform))
        {
            if (_timing.CurTime < rotting.NextRotUpdate) // This is where it starts to get noticable on larger animals, no need to run every second
                continue;
            rotting.NextRotUpdate += rotting.RotUpdateRate;

            if (!IsRotProgressing(uid, perishable))
                continue;
            rotting.TotalRotTime += rotting.RotUpdateRate * GetRotRate(uid);

            if (rotting.DealDamage)
            {
                var damage = rotting.Damage * rotting.RotUpdateRate.TotalSeconds;
                _damageable.TryChangeDamage(uid, damage, true, false);
            }

            if (TryComp<RotIntoComponent>(uid, out var rotInto))
            {
                var stage = RotStage(uid, rotting, perishable);
                if (stage >= rotInto.Stage)
                {
                    Spawn(rotInto.Entity, xform.Coordinates);
                    QueueDel(uid);
                    continue;
                }
            }

            if (!TryComp<PhysicsComponent>(uid, out var physics))
                continue;
            // We need a way to get the mass of the mob alone without armor etc in the future
            // or just remove the mass mechanics altogether because they aren't good.
            var molRate = perishable.MolsPerSecondPerUnitMass * (float)rotting.RotUpdateRate.TotalSeconds;
            var tileMix = _atmosphere.GetTileMixture(uid, excite: true);
            tileMix?.AdjustMoles(Gas.Ammonia, molRate * physics.FixturesMass);
        }
    }
}
