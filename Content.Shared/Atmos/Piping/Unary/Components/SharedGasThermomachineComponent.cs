// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2023 Kevin Zheng
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Unary.Components;

[Serializable, NetSerializable]
public sealed record GasThermoMachineData(float EnergyDelta);

[Serializable]
[NetSerializable]
public enum ThermomachineUiKey : byte
{
    Key
}

[Serializable]
[NetSerializable]
public sealed class GasThermomachineToggleMessage : BoundUserInterfaceMessage
{
}

[Serializable]
[NetSerializable]
public sealed class GasThermomachineChangeTemperatureMessage : BoundUserInterfaceMessage
{
    public float Temperature { get; }

    public GasThermomachineChangeTemperatureMessage(float temperature)
    {
        Temperature = temperature;
    }
}
