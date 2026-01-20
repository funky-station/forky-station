// SPDX-FileCopyrightText: 2023 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 T-Stalker
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 20kdc
// SPDX-FileCopyrightText: 2020 Swept
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Dataset;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Humanoid
{
    // You need to update profile, profile editor, maybe voices and names if you want to expand this further.
    public enum Sex : byte
    {
        Male,
        Female,
        Unsexed,
    }

    /// <summary>
    ///     Raised when entity has changed their sex.
    ///     This doesn't handle gender changes.
    /// </summary>
    public record struct SexChangedEvent(Sex OldSex, Sex NewSex);
}
