// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 ike709
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable, NetSerializable]
public enum GasPressurePumpUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class GasPressurePumpToggleStatusMessage(bool enabled) : BoundUserInterfaceMessage
{
    public bool Enabled { get; } = enabled;
}

[Serializable, NetSerializable]
public sealed class GasPressurePumpChangeOutputPressureMessage(float pressure) : BoundUserInterfaceMessage
{
    public float Pressure { get; } = pressure;
}
