// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-License-Identifier: MIT

using Robust.Shared.Threading;

namespace Content.Server.Power.Pow3r
{
    public sealed class NoOpSolver : IPowerSolver
    {
        public void Tick(float frameTime, PowerState state, IParallelManager parallel)
        {
            // Literally nothing.
        }

        public void Validate(PowerState state)
        {
            // Literally nothing.
        }
    }
}
