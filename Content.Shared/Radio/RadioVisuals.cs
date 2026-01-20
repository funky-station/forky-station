// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Radio;

[Serializable, NetSerializable]
public enum RadioDeviceVisuals : byte
{
    Broadcasting,
    Speaker
}

[Serializable, NetSerializable]
public enum RadioDeviceVisualLayers : byte
{
    Broadcasting,
    Speaker
}
