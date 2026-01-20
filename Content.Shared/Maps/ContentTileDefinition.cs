// SPDX-FileCopyrightText: 2026 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2026 Velken
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 Ilya246
// SPDX-FileCopyrightText: 2025 Ilya246
// SPDX-FileCopyrightText: 2025 SlamBamActionman
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2025 metalgearsloth
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2023 Vordenburg
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 20kdc
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Morb
// SPDX-FileCopyrightText: 2022 Jacob Tong
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-FileCopyrightText: 2021 Kara D
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 moonheart08
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Rohesie
// SPDX-FileCopyrightText: 2019 Ephememory
// SPDX-FileCopyrightText: 2019 Injazz
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2019 PrPleGoo
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos;
using Content.Shared.Light.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;

namespace Content.Shared.Maps
{
    [Prototype("tile")]
    public sealed partial class ContentTileDefinition : IPrototype, IInheritingPrototype, ITileDefinition
    {
        public static readonly ProtoId<ToolQualityPrototype> PryingToolQuality = "Prying";

        public const string SpaceID = "Space";

        [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ContentTileDefinition>))]
        public string[]? Parents { get; private set; }

        [NeverPushInheritance]
        [AbstractDataFieldAttribute]
        public bool Abstract { get; private set; }

        [IdDataField] public string ID { get; private set; } = string.Empty;

        public ushort TileId { get; private set; }

        [DataField("name")]
        public string Name { get; private set; } = "";
        [DataField("sprite")] public ResPath? Sprite { get; private set; }

        [DataField("edgeSprites")] public Dictionary<Direction, ResPath> EdgeSprites { get; private set; } = new();

        [DataField("edgeSpritePriority")] public int EdgeSpritePriority { get; private set; } = 0;

        [DataField("isSubfloor")] public bool IsSubFloor { get; private set; }

        [DataField("baseTurf")]
        public string BaseTurf { get; private set; } = string.Empty;

        [DataField]
        public PrototypeFlags<ToolQualityPrototype> DeconstructTools { get; set; } = new();

        /// <summary>
        /// Effective mass of this tile for grid impacts.
        /// </summary>
        [DataField]
        public float Mass = 800f;

        /// <remarks>
        /// Legacy AF but nice to have.
        /// </remarks>
        public bool CanCrowbar => DeconstructTools.Contains(PryingToolQuality);

        /// <summary>
        /// These play when the mob has shoes on.
        /// </summary>
        [DataField("footstepSounds")] public SoundSpecifier? FootstepSounds { get; private set; }

        /// <summary>
        /// These play when the mob has no shoes on.
        /// </summary>
        [DataField("barestepSounds")] public SoundSpecifier? BarestepSounds { get; private set; } = new SoundCollectionSpecifier("BarestepHard");

        /// <summary>
        /// Base friction modifier for this tile.
        /// </summary>
        [DataField("friction")] public float Friction { get; set; } = 1f;

        [DataField("variants")] public byte Variants { get; set; } = 1;

        /// <summary>
        ///     Allows the tile to be rotated/mirrored when placed on a grid.
        /// </summary>
        [DataField] public bool AllowRotationMirror { get; set; } = false;

        /// <summary>
        /// This controls what variants the `variantize` command is allowed to use.
        /// </summary>
        [DataField("placementVariants")] public float[] PlacementVariants { get; set; } = { 1f };

        [DataField("thermalConductivity")] public float ThermalConductivity = 0.04f;

        // Heat capacity is opt-in, not opt-out.
        [DataField("heatCapacity")] public float HeatCapacity = Atmospherics.MinimumHeatCapacity;

        [DataField("itemDrop", customTypeSerializer:typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string ItemDropPrototypeName { get; private set; } = "FloorTileItemSteel";

        // TODO rename data-field in yaml
        /// <summary>
        /// Whether or not the tile is exposed to the map's atmosphere.
        /// </summary>
        [DataField("isSpace")] public bool MapAtmosphere { get; private set; }

        /// <summary>
        ///     Friction override for mob mover in <see cref="SharedMoverController"/>
        /// </summary>
        [DataField("mobFriction")]
        public float? MobFriction { get; private set; }

        /// <summary>
        ///     Accel override for mob mover in <see cref="SharedMoverController"/>
        /// </summary>
        [DataField("mobAcceleration")]
        public float? MobAcceleration { get; private set; }

        [DataField("sturdy")] public bool Sturdy { get; private set; } = true;

        /// <summary>
        /// Can weather affect this tile.
        /// </summary>
        [DataField("weather")] public bool Weather = false;

        /// <summary>
        /// Is this tile immune to RCD deconstruct.
        /// </summary>
        [DataField("indestructible")] public bool Indestructible = false;

        public void AssignTileId(ushort id)
        {
            TileId = id;
        }
    }
}
