// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2023 Ilushkins33
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Acruid
// SPDX-FileCopyrightText: 2021 pointer-to-null
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.StatusEffectNew;
using Robust.Shared.Prototypes;

namespace Content.Shared.Speech.EntitySystems;

public abstract class SharedStutteringSystem : EntitySystem
{
    public static readonly EntProtoId Stuttering = "StatusEffectStutter";

    [Dependency] protected readonly StatusEffectsSystem Status = default!;

    // For code in shared... I imagine we ain't getting accent prediction anytime soon so let's not bother.
    public virtual void DoStutter(EntityUid uid, TimeSpan time, bool refresh)
    {
    }

    public virtual void DoRemoveStutterTime(EntityUid uid, TimeSpan timeRemoved)
    {
    }

    public virtual void DoRemoveStutter(EntityUid uid)
    {
    }
}
