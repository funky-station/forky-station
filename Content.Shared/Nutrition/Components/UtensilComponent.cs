// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2025 DrSmugleaf
// SPDX-FileCopyrightText: 2024 Magnus Larsen
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 FoLoKe
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2020 DmitriyRubetskoy
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-License-Identifier: MIT

using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Nutrition.Components
{
    [RegisterComponent, NetworkedComponent, Access(typeof(IngestionSystem))]
    public sealed partial class UtensilComponent : Component
    {
        [DataField("types")]
        private UtensilType _types = UtensilType.None;

        [ViewVariables]
        public UtensilType Types
        {
            get => _types;
            set
            {
                if (_types.Equals(value))
                    return;

                _types = value;
            }
        }

        /// <summary>
        /// The chance that the utensil has to break with each use.
        /// A value of 0 means that it is unbreakable.
        /// </summary>
        [DataField("breakChance")]
        public float BreakChance;

        /// <summary>
        /// The sound to be played if the utensil breaks.
        /// </summary>
        [DataField("breakSound")]
        public SoundSpecifier BreakSound = new SoundPathSpecifier("/Audio/Items/snap.ogg");
    }

    // If you want to make a fancy output on "wrong" composite utensil use (like: you need fork and knife)
    // There should be Dictionary I guess (Dictionary<UtensilType, string>)
    [Flags]
    public enum UtensilType : byte
    {
        None = 0,
        Fork = 1,
        Spoon = 1 << 1,
        Knife = 1 << 2
    }
}
