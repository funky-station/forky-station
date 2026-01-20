// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.StatusEffect;
using Robust.Shared.Prototypes;

namespace Content.Shared.Speech.EntitySystems;

public abstract class SharedSlurredSystem : EntitySystem
{
    public static readonly EntProtoId Stutter = "StatusEffectSlurred";

    public virtual void DoSlur(EntityUid uid, TimeSpan time, StatusEffectsComponent? status = null) { }
}
