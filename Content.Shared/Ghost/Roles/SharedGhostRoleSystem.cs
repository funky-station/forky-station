// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable, NetSerializable]
public sealed class GhostRole
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public NetEntity Id;
}
