// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2024 Plykiya
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Errant
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2022 themias
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Matz05
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 Julian Giebel
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2021 Silver
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2019 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 py01
// SPDX-FileCopyrightText: 2021 py01
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 RemberBL
// SPDX-FileCopyrightText: 2020 ShadowCommander
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Memory
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2020 Clyybber
// SPDX-FileCopyrightText: 2019 PrPleGoo
// SPDX-License-Identifier: MIT

using Content.Shared.DeviceLinking;
using Content.Shared.Light.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Light.Components
{
    /// <summary>
    ///     Component that represents a wall light. It has a light bulb that can be replaced when broken.
    /// </summary>
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause, Access(typeof(SharedPoweredLightSystem))]
    public sealed partial class PoweredLightComponent : Component
    {
        /*
         * Stop adding more fields, use components or I will shed you.
         */

        [DataField]
        public SoundSpecifier BurnHandSound = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg");

        [DataField]
        public SoundSpecifier TurnOnSound = new SoundPathSpecifier("/Audio/Machines/light_tube_on.ogg");

        // Should be using containerfill?
        [DataField]
        public EntProtoId? HasLampOnSpawn = null;

        [DataField("bulb")]
        public LightBulbType BulbType;

        [DataField, AutoNetworkedField]
        public bool On = true;

        [DataField]
        public bool IgnoreGhostsBoo;

        [DataField]
        public TimeSpan GhostBlinkingTime = TimeSpan.FromSeconds(10);

        [DataField]
        public TimeSpan GhostBlinkingCooldown = TimeSpan.FromSeconds(60);

        [ViewVariables]
        public ContainerSlot LightBulbContainer = default!;

        [AutoNetworkedField]
        public bool CurrentLit;

        [DataField, AutoNetworkedField]
        public bool IsBlinking;

        [DataField, AutoNetworkedField, AutoPausedField]
        public TimeSpan LastThunk;

        [DataField, AutoPausedField]
        public TimeSpan? LastGhostBlink;

        [DataField]
        public ProtoId<SinkPortPrototype> OnPort = "On";

        [DataField]
        public ProtoId<SinkPortPrototype> OffPort = "Off";

        [DataField]
        public ProtoId<SinkPortPrototype> TogglePort = "Toggle";

        /// <summary>
        /// How long it takes to eject a bulb from this
        /// </summary>
        [DataField]
        public float EjectBulbDelay = 2;

        /// <summary>
        /// Shock damage done to a mob that hits the light with an unarmed attack
        /// </summary>
        [DataField]
        public int UnarmedHitShock = 20;

        /// <summary>
        /// Stun duration applied to a mob that hits the light with an unarmed attack
        /// </summary>
        [DataField]
        public TimeSpan UnarmedHitStun = TimeSpan.FromSeconds(5);
    }
}
