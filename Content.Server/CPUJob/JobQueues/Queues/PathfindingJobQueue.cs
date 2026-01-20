// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.CPUJob.JobQueues.Queues;

namespace Content.Server.CPUJob.JobQueues.Queues
{
    public sealed class PathfindingJobQueue : JobQueue
    {
        public override double MaxTime => 0.003;
    }
}
