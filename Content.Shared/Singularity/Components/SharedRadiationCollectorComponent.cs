// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
    [NetSerializable, Serializable]
    public enum RadiationCollectorVisuals
    {
        VisualState,
        TankInserted,
        PressureState,
    }

    [NetSerializable, Serializable]
    public enum RadiationCollectorVisualState
    {
        Active = (1<<0),
        Activating = (1<<1) | Active,
        Deactivating = (1<<1),
        Deactive = 0
    }
}
