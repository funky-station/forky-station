// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 Ciarán Walsh
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2024 Stalen
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2024 Jake Huxell
// SPDX-FileCopyrightText: 2024 0x6273
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Justin Trotter
// SPDX-FileCopyrightText: 2022 Bright0
// SPDX-FileCopyrightText: 2022 corentt
// SPDX-FileCopyrightText: 2022 Kognise
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Popups;
using Content.Shared.Alert;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Internals;
using Content.Shared.Inventory;
using Content.Shared.Roles;

namespace Content.Server.Body.Systems;

public sealed class InternalsSystem : SharedInternalsSystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly GasTankSystem _gasTank = default!;
    [Dependency] private readonly RespiratorSystem _respirator = default!;

    private EntityQuery<InternalsComponent> _internalsQuery;

    public override void Initialize()
    {
        base.Initialize();

        _internalsQuery = GetEntityQuery<InternalsComponent>();

        SubscribeLocalEvent<InternalsComponent, InhaleLocationEvent>(OnInhaleLocation);
        SubscribeLocalEvent<InternalsComponent, StartingGearEquippedEvent>(OnStartingGear);
    }

    private void OnStartingGear(EntityUid uid, InternalsComponent component, ref StartingGearEquippedEvent args)
    {
        if (component.BreathTools.Count == 0)
            return;

        if (component.GasTankEntity != null)
            return; // already connected

        // Can the entity breathe the air it is currently exposed to?
        if (_respirator.CanMetabolizeInhaledAir(uid))
            return;

        var tank = FindBestGasTank(uid);
        if (tank == null)
            return;

        // Could the entity metabolise the air in the linked gas tank?
        if (!_respirator.CanMetabolizeInhaledAir(uid, tank.Value.Comp.Air))
            return;

        ToggleInternals(uid, uid, force: false, component, ToggleMode.On);
    }

    private void OnInhaleLocation(Entity<InternalsComponent> ent, ref InhaleLocationEvent args)
    {
        if (AreInternalsWorking(ent))
        {
            var gasTank = Comp<GasTankComponent>(ent.Comp.GasTankEntity!.Value);
            args.Gas = _gasTank.RemoveAirVolume((ent.Comp.GasTankEntity.Value, gasTank), args.Respirator.BreathVolume);
            // TODO: Should listen to gas tank updates instead I guess?
            _alerts.ShowAlert(ent.Owner, ent.Comp.InternalsAlert, GetSeverity(ent));
        }
    }
}
