// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Flash;

[Serializable, NetSerializable]
public enum FlashVisuals : byte
{
    Burnt,
    Flashing,
}

[Serializable, NetSerializable]
public enum FlashVisualLayers : byte
{
    BaseLayer,
    LightLayer,
}
