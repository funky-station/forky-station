// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Clothing.Components;
using Robust.Client.Physics;

namespace Content.Client.Clothing.Systems;

public sealed partial class PilotedByClothingSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PilotedByClothingComponent, UpdateIsPredictedEvent>(OnUpdatePredicted);
    }

    private void OnUpdatePredicted(Entity<PilotedByClothingComponent> entity, ref UpdateIsPredictedEvent args)
    {
        args.BlockPrediction = true;
    }
}
