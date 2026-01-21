// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 coolmankid12345 <55817627+coolmankid12345@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Connor Huffine <chuffine@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Administration.Logs;
using Content.Server.Mind;
using Content.Server.Popups;
using Content.Server.Roles;
using Content.Shared.Database;
using Content.Shared.Implants;
using Content.Shared.Mindshield.Components;
using Content.Shared.Revolutionary.Components;
using Content.Shared.Roles.Components;
using Robust.Shared.Containers;

namespace Content.Server.Mindshield;

/// <summary>
/// System used for adding or removing components with a mindshield implant
/// as well as checking if the implanted is a Rev or Head Rev.
/// </summary>
public sealed class MindShieldSystem : EntitySystem
{
    [Dependency] private readonly IAdminLogManager _adminLogManager = default!;
    [Dependency] private readonly RoleSystem _roleSystem = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MindShieldImplantComponent, ImplantImplantedEvent>(OnImplantImplanted);
        SubscribeLocalEvent<MindShieldImplantComponent, ImplantRemovedEvent>(OnImplantRemoved);
    }

    private void OnImplantImplanted(Entity<MindShieldImplantComponent> ent, ref ImplantImplantedEvent ev)
    {
        EnsureComp<MindShieldComponent>(ev.Implanted);
        MindShieldRemovalCheck(ev.Implanted, ev.Implant);
    }

    /// <summary>
    /// Checks if the implanted person was a Rev or Head Rev and remove role or destroy mindshield respectively.
    /// </summary>
    private void MindShieldRemovalCheck(EntityUid implanted, EntityUid implant)
    {
        if (HasComp<HeadRevolutionaryComponent>(implanted))
        {
            _popupSystem.PopupEntity(Loc.GetString("head-rev-break-mindshield"), implanted);
            QueueDel(implant);
            return;
        }

        if (_mindSystem.TryGetMind(implanted, out var mindId, out _) &&
            _roleSystem.MindRemoveRole<RevolutionaryRoleComponent>(mindId))
        {
            _adminLogManager.Add(LogType.Mind, LogImpact.Medium, $"{ToPrettyString(implanted)} was deconverted due to being implanted with a Mindshield.");
        }
    }

    private void OnImplantRemoved(Entity<MindShieldImplantComponent> ent, ref ImplantRemovedEvent args)
    {
        RemComp<MindShieldComponent>(args.Implanted);
    }
}

