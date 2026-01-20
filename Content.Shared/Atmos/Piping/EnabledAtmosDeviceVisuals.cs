// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2023 Menshin
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping
{
    [Serializable, NetSerializable]
    public enum OutletInjectorVisuals : byte
    {
        Enabled,
    }

    [Serializable, NetSerializable]
    public enum PassiveVentVisuals : byte
    {
        Enabled,
    }

    [Serializable, NetSerializable]
    public enum VentScrubberVisuals : byte
    {
        Enabled,
    }

    [Serializable, NetSerializable]
    public enum PumpVisuals : byte
    {
        Enabled,
    }

    [Serializable, NetSerializable]
    public enum FilterVisuals : byte
    {
        Enabled,
    }

    [Serializable, NetSerializable]
    public enum PressureRegulatorVisuals : byte
    {
        State,
    }
}
