// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2023 Emisse
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Shared.Eui;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Administration
{
    [Serializable, NetSerializable]
    public sealed class EditSolutionsEuiState : EuiStateBase
    {
        public readonly NetEntity Target;
        public readonly List<(string, NetEntity)>? Solutions;
        public readonly GameTick Tick;

        public EditSolutionsEuiState(NetEntity target, List<(string, NetEntity)>? solutions, GameTick tick)
        {
            Target = target;
            Solutions = solutions;
            Tick = tick;
        }
    }
}
