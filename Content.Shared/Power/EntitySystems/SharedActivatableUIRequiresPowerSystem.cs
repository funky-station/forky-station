// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Power.Components;
using Content.Shared.UserInterface;

namespace Content.Shared.Power.EntitySystems;

public abstract class SharedActivatableUIRequiresPowerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ActivatableUIRequiresPowerComponent, ActivatableUIOpenAttemptEvent>(OnActivate);
    }

    protected abstract void OnActivate(Entity<ActivatableUIRequiresPowerComponent> ent, ref ActivatableUIOpenAttemptEvent args);
}
