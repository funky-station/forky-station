// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Spreader;

[Serializable, NetSerializable]
public enum KudzuVisuals : byte
{
    GrowthLevel,
    Variant
}
