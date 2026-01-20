// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Julian Giebel
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-License-Identifier: MIT


using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.DeviceLinking.Components
{
    [RegisterComponent]
    public sealed partial class ActiveSignalTimerComponent : Component
    {
        /// <summary>
        ///     The time the timer triggers.
        /// </summary>
        [DataField("triggerTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
        public TimeSpan TriggerTime;
    }
}
