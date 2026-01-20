// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable.EntitySelectors;
using JetBrains.Annotations;

namespace Content.Shared.EntityTable.ValueSelector;

/// <summary>
/// Used for implementing custom value selection for <see cref="EntityTableSelector"/>
/// </summary>
[ImplicitDataDefinitionForInheritors, UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract partial class NumberSelector
{
    public abstract int Get(System.Random rand);
}
