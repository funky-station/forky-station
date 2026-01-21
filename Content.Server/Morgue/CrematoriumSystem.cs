// SPDX-FileCopyrightText: 2022-2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Winkarst <74284083+Winkarst-cpu@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Scribbles0 <91828755+Scribbles0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Ghost;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Morgue;
using Content.Shared.Morgue.Components;
using Content.Shared.Popups;
using Robust.Shared.Player;

namespace Content.Server.Morgue;
public sealed class CrematoriumSystem : SharedCrematoriumSystem
{
    [Dependency] private readonly GhostSystem _ghostSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CrematoriumComponent, SuicideByEnvironmentEvent>(OnSuicideByEnvironment);
    }

    private void OnSuicideByEnvironment(Entity<CrematoriumComponent> ent, ref SuicideByEnvironmentEvent args)
    {
        if (args.Handled)
            return;

        var victim = args.Victim;
        if (HasComp<ActorComponent>(victim) && Mind.TryGetMind(victim, out var mindId, out var mind))
        {
            _ghostSystem.OnGhostAttempt(mindId, false, mind: mind);

            if (mind.OwnedEntity is { Valid: true } entity)
            {
                Popup.PopupEntity(Loc.GetString("crematorium-entity-storage-component-suicide-message"), entity);
            }
        }

        Popup.PopupEntity(Loc.GetString("crematorium-entity-storage-component-suicide-message-others",
            ("victim", Identity.Entity(victim, EntityManager))),
            victim,
            Filter.PvsExcept(victim),
            true,
            PopupType.LargeCaution);

        if (EntityStorage.CanInsert(victim, ent.Owner))
        {
            EntityStorage.CloseStorage(ent.Owner);
            Standing.Down(victim, false);
            EntityStorage.Insert(victim, ent.Owner);
        }
        else
        {
            EntityStorage.CloseStorage(ent.Owner);
            Del(victim);
        }
        Cremate(ent.AsNullable());
        args.Handled = true;
    }
}
