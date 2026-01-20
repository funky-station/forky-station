// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Ed
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-License-Identifier: MIT

using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.UserInterface;
using ActivatableUISystem = Content.Shared.UserInterface.ActivatableUISystem;

namespace Content.Server.Power.EntitySystems;

public sealed class ActivatableUIRequiresPowerSystem : SharedActivatableUIRequiresPowerSystem
{
    [Dependency] private readonly ActivatableUISystem _activatableUI = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActivatableUIRequiresPowerComponent, PowerChangedEvent>(OnPowerChanged);
    }

    protected override void OnActivate(Entity<ActivatableUIRequiresPowerComponent> ent, ref ActivatableUIOpenAttemptEvent args)
    {
        if (args.Cancelled || this.IsPowered(ent.Owner, EntityManager))
        {
            return;
        }

        args.Cancel();
    }

    private void OnPowerChanged(EntityUid uid, ActivatableUIRequiresPowerComponent component, ref PowerChangedEvent args)
    {
        if (!args.Powered)
            _activatableUI.CloseAll(uid);
    }
}
