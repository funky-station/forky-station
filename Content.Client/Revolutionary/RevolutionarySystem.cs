// SPDX-FileCopyrightText: 2023 coolmankid12345 <55817627+coolmankid12345@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 nikthechampiongr <32041239+nikthechampiongr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Revolutionary.Components;
using Content.Shared.Revolutionary;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.Revolutionary;

/// <summary>
/// Used for the client to get status icons from other revs.
/// </summary>
public sealed class RevolutionarySystem : SharedRevolutionarySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RevolutionaryComponent, GetStatusIconsEvent>(GetRevIcon);
        SubscribeLocalEvent<HeadRevolutionaryComponent, GetStatusIconsEvent>(GetHeadRevIcon);
    }

    private void GetRevIcon(Entity<RevolutionaryComponent> ent, ref GetStatusIconsEvent args)
    {
        if (HasComp<HeadRevolutionaryComponent>(ent))
            return;

        if (_prototype.Resolve(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }

    private void GetHeadRevIcon(Entity<HeadRevolutionaryComponent> ent, ref GetStatusIconsEvent args)
    {
        if (_prototype.Resolve(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }
}
