// SPDX-FileCopyrightText: 2024 0x6273
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-License-Identifier: MIT

namespace Content.Shared.Body.Events;

[ByRefEvent]
public record struct ShiverAttemptEvent(EntityUid Uid)
{
    public readonly EntityUid Uid = Uid;
    public bool Cancelled = false;
}
