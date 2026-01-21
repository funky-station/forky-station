// SPDX-FileCopyrightText: 2023-2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023-2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Repo <47093363+Titian3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 KP <13428215+nok-ko@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 LightVillet <maxim12000@ya.ru>
// SPDX-FileCopyrightText: 2024 Cojoke <83733158+Cojoke-dot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 Vasilis <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2024 nikthechampiongr <32041239+nikthechampiongr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Milon <milonpl.git@proton.me>
// SPDX-FileCopyrightText: 2025 Perry Fraser <perryprog@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Database;
using Content.Shared.Fluids.Components;
using Content.Shared.Spillable;
using Content.Shared.Throwing;

namespace Content.Server.Fluids.EntitySystems;

public sealed partial class PuddleSystem
{
    protected override void InitializeSpillable()
    {
        base.InitializeSpillable();

        SubscribeLocalEvent<SpillableComponent, LandEvent>(SpillOnLand);
        // Openable handles the event if it's closed
        SubscribeLocalEvent<SpillableComponent, SolutionContainerOverflowEvent>(OnOverflow);
        SubscribeLocalEvent<SpillableComponent, SpillDoAfterEvent>(OnDoAfter);
    }

    private void OnOverflow(Entity<SpillableComponent> entity, ref SolutionContainerOverflowEvent args)
    {
        if (args.Handled)
            return;

        TrySpillAt(Transform(entity).Coordinates, args.Overflow, out _);
        args.Handled = true;
    }

    private void SpillOnLand(Entity<SpillableComponent> entity, ref LandEvent args)
    {
        if (!entity.Comp.SpillWhenThrown || Openable.IsClosed(entity.Owner))
            return;

        if (TrySplashSpillAt(entity.Owner, Transform(entity).Coordinates, out _, out var solution) && args.User != null)
        {
            AdminLogger.Add(LogType.Landed,
                $"{ToPrettyString(entity.Owner):entity} spilled a solution {SharedSolutionContainerSystem.ToPrettyString(solution):solution} on landing");
        }
    }

    private void OnDoAfter(Entity<SpillableComponent> entity, ref SpillDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Args.Target == null)
            return;

        //solution gone by other means before doafter completes
        if (!_solutionContainerSystem.TryGetDrainableSolution(entity.Owner, out var soln, out var solution) || solution.Volume == 0)
            return;

        var puddleSolution = _solutionContainerSystem.SplitSolution(soln.Value, solution.Volume);
        TrySpillAt(Transform(entity).Coordinates, puddleSolution, out _);
        args.Handled = true;
    }
}
