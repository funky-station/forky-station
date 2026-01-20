// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Shared.Decals;

namespace Content.MapRenderer.Painters;

public readonly record struct DecalData(Decal Decal, float X, float Y);
