// SPDX-FileCopyrightText: 2024 Menshin <Menshin@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 Steve <marlumpy@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Atmos.Visuals;

/// <summary>
///     Funky atmos - /tg/ gases
///     Used for the visualizer
/// </summary>
[Serializable, NetSerializable]
public enum ElectrolyzerVisualLayers : byte
{
    Main
}

[Serializable, NetSerializable]
public enum ElectrolyzerVisuals : byte
{
    State,
}

[Serializable, NetSerializable]
public enum ElectrolyzerState : byte
{
    Off,
    On,
}
