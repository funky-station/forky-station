// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Jessica M
// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Revenant;

[Serializable, NetSerializable]
public sealed partial class SoulEvent : SimpleDoAfterEvent
{
}

public sealed class SoulSearchDoAfterComplete : EntityEventArgs
{
    public readonly EntityUid Target;

    public SoulSearchDoAfterComplete(EntityUid target)
    {
        Target = target;
    }
}

public sealed class SoulSearchDoAfterCancelled : EntityEventArgs
{
}

[Serializable, NetSerializable]
public sealed partial class HarvestEvent : SimpleDoAfterEvent
{
}

public sealed class HarvestDoAfterComplete : EntityEventArgs
{
    public readonly EntityUid Target;

    public HarvestDoAfterComplete(EntityUid target)
    {
        Target = target;
    }
}

public sealed class HarvestDoAfterCancelled : EntityEventArgs
{
}

public sealed partial class RevenantDefileActionEvent : InstantActionEvent
{
}

public sealed partial class RevenantOverloadLightsActionEvent : InstantActionEvent
{
}

public sealed partial class RevenantBlightActionEvent : InstantActionEvent
{
}

public sealed partial class RevenantMalfunctionActionEvent : InstantActionEvent
{
}


[NetSerializable, Serializable]
public enum RevenantVisuals : byte
{
    Corporeal,
    Stunned,
    Harvesting,
}
