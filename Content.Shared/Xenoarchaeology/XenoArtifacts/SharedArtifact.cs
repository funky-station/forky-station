// SPDX-FileCopyrightText: 2025 Fildrance
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Xenoarchaeology.XenoArtifacts;

[Serializable, NetSerializable]
public enum SharedArtifactsVisuals : byte
{
    SpriteIndex,
    IsActivated,
    IsUnlocking
}
