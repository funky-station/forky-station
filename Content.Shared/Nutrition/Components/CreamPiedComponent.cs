// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Serialization;

namespace Content.Shared.Nutrition.Components
{
    [Access(typeof(SharedCreamPieSystem))]
    [RegisterComponent]
    public sealed partial class CreamPiedComponent : Component
    {
        [ViewVariables]
        public bool CreamPied { get; set; } = false;
    }

    [Serializable, NetSerializable]
    public enum CreamPiedVisuals
    {
        Creamed,
    }
}
