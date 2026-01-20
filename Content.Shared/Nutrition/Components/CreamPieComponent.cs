// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Audio;

namespace Content.Shared.Nutrition.Components
{
    [Access(typeof(SharedCreamPieSystem))]
    [RegisterComponent]
    public sealed partial class CreamPieComponent : Component
    {
        [DataField("paralyzeTime")]
        public float ParalyzeTime { get; private set; } = 1f;

        [DataField("sound")]
        public SoundSpecifier Sound { get; private set; } = new SoundCollectionSpecifier("desecration");

        [ViewVariables]
        public bool Splatted { get; set; } = false;

        public const string PayloadSlotName = "payloadSlot";
    }
}
