// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 SlamBamActionman
// SPDX-License-Identifier: MIT

using Content.Shared.StepTrigger.Systems;
using Content.Shared.EntityEffects;

namespace Content.Server.Tiles;

public sealed class TileEntityEffectSystem : EntitySystem
{
    [Dependency] private readonly SharedEntityEffectsSystem _entityEffects = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TileEntityEffectComponent, StepTriggeredOffEvent>(OnTileStepTriggered);
        SubscribeLocalEvent<TileEntityEffectComponent, StepTriggerAttemptEvent>(OnTileStepTriggerAttempt);
    }
    private void OnTileStepTriggerAttempt(Entity<TileEntityEffectComponent> ent, ref StepTriggerAttemptEvent args)
    {
        args.Continue = true;
    }

    private void OnTileStepTriggered(Entity<TileEntityEffectComponent> ent, ref StepTriggeredOffEvent args)
    {
        var otherUid = args.Tripper;

        _entityEffects.ApplyEffects(otherUid, ent.Comp.Effects.ToArray(), user: otherUid);
    }
}
