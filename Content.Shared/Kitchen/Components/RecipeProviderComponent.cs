// SPDX-FileCopyrightText: 2024 themias <89101928+themias@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;

namespace Content.Shared.Kitchen.Components;

[RegisterComponent]
public sealed partial class FoodRecipeProviderComponent : Component
{
    /// <summary>
    /// These are additional recipes that the entity is capable of cooking.
    /// </summary>
    [DataField, ViewVariables]
    public List<ProtoId<FoodRecipePrototype>> ProvidedRecipes = new();
}
