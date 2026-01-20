// SPDX-FileCopyrightText: 2024 themias
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2022 Kevin Zheng
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2020 FLOZ
// SPDX-License-Identifier: MIT

using System.Linq;
using Robust.Shared.Prototypes;

namespace Content.Shared.Kitchen
{
    public sealed class RecipeManager
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public List<FoodRecipePrototype> Recipes { get; private set; } = new();

        public void Initialize()
        {
            Recipes = new List<FoodRecipePrototype>();
            foreach (var item in _prototypeManager.EnumeratePrototypes<FoodRecipePrototype>())
            {
                if (!item.SecretRecipe)
                    Recipes.Add(item);
            }

            Recipes.Sort(new RecipeComparer());
        }
        /// <summary>
        /// Check if a prototype ids appears in any of the recipes that exist.
        /// </summary>
        public bool SolidAppears(string solidId)
        {
            return Recipes.Any(recipe => recipe.IngredientsSolids.ContainsKey(solidId));
        }

        private sealed class RecipeComparer : Comparer<FoodRecipePrototype>
        {
            public override int Compare(FoodRecipePrototype? x, FoodRecipePrototype? y)
            {
                if (x == null || y == null)
                {
                    return 0;
                }

                var nx = x.IngredientCount();
                var ny = y.IngredientCount();
                return -nx.CompareTo(ny);
            }
        }
    }
}
