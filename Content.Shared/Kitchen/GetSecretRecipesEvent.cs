// SPDX-FileCopyrightText: 2024 themias <89101928+themias@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Shared.Kitchen;

/// <summary>
/// This returns a list of recipes not found in the main list of available recipes.
/// </summary>
[ByRefEvent]
public struct GetSecretRecipesEvent()
{
    public List<FoodRecipePrototype> Recipes = new();
}
