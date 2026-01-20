// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-FileCopyrightText: 2022 vulppine
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Monitor;

[Serializable, NetSerializable]
public enum AtmosAlarmType : sbyte
{
    Invalid = 0,
    Normal = 1,
    Warning = 2,
    Danger = 3,
    Emagged = 4,
}
