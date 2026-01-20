// SPDX-FileCopyrightText: 2024 LordCarve
// SPDX-FileCopyrightText: 2024 Sergey Dikiy
// SPDX-FileCopyrightText: 2023 daerSeebaer
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Ame.Components;

[Virtual]
public partial class SharedAmeControllerComponent : Component
{
    public const string FuelSlotId = "fuelSlot";
}

[Serializable, NetSerializable]
public sealed class AmeControllerBoundUserInterfaceState : BoundUserInterfaceState
{
    public readonly bool HasPower;
    public readonly bool IsMaster;
    public readonly bool Injecting;
    public readonly bool HasFuelJar;
    public readonly int FuelAmount;
    public readonly int InjectionAmount;
    public readonly int CoreCount;
    public readonly float CurrentPowerSupply;
    public readonly float TargetedPowerSupply;

    public AmeControllerBoundUserInterfaceState(bool hasPower, bool isMaster, bool injecting, bool hasFuelJar, int fuelAmount, int injectionAmount, int coreCount, float currentPowerSupply, float targetedPowerSupply)
    {
        HasPower = hasPower;
        IsMaster = isMaster;
        Injecting = injecting;
        HasFuelJar = hasFuelJar;
        FuelAmount = fuelAmount;
        InjectionAmount = injectionAmount;
        CoreCount = coreCount;
        CurrentPowerSupply = currentPowerSupply;
        TargetedPowerSupply = targetedPowerSupply;
    }
}

[Serializable, NetSerializable]
public sealed class UiButtonPressedMessage : BoundUserInterfaceMessage
{
    public readonly UiButton Button;

    public UiButtonPressedMessage(UiButton button)
    {
        Button = button;
    }
}

[Serializable, NetSerializable]
public enum AmeControllerUiKey
{
    Key
}

public enum UiButton
{
    Eject,
    ToggleInjection,
    IncreaseFuel,
    DecreaseFuel,
}

[Serializable, NetSerializable]
public enum AmeControllerVisuals
{
    DisplayState,
}

[Serializable, NetSerializable]
public enum AmeControllerState
{
    On,
    Warning,
    Critical,
    Fuck,
    Off,
}
