// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Components;

[Serializable, NetSerializable]
public enum SharedGasTankUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class GasTankToggleInternalsMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class GasTankSetPressureMessage : BoundUserInterfaceMessage
{
    public float Pressure;
}

[Serializable, NetSerializable]
public sealed class GasTankBoundUserInterfaceState : BoundUserInterfaceState
{
    public float TankPressure;
}
