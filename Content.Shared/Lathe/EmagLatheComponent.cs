// SPDX-FileCopyrightText: 2025 deltanedas
// SPDX-FileCopyrightText: 2024 Ilya246
// SPDX-FileCopyrightText: 2023 ubis1
// SPDX-License-Identifier: MIT

using Content.Shared.Lathe.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Lathe
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class EmagLatheRecipesComponent : Component
    {
        /// <summary>
        /// All of the dynamic recipe packs that the lathe is capable to get using EMAG
        /// </summary>
        [DataField, AutoNetworkedField]
        public List<ProtoId<LatheRecipePackPrototype>> EmagDynamicPacks = new();

        /// <summary>
        /// All of the static recipe packs that the lathe is capable to get using EMAG
        /// </summary>
        [DataField, AutoNetworkedField]
        public List<ProtoId<LatheRecipePackPrototype>> EmagStaticPacks = new();
    }
}
