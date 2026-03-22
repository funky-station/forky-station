// SPDX-FileCopyrightText: 2022, 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022-2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ArtisticRoomba <145879011+ArtisticRoomba@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 Steve <marlumpy@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Atmos;
using Content.Shared.Guidebook;

namespace Content.Server.Atmos.Portable
{
    [RegisterComponent]
    public sealed partial class PortableScrubberComponent : Component
    {
        /// <summary>
        /// The air inside this machine.
        /// </summary>
        [DataField("gasMixture"), ViewVariables(VVAccess.ReadWrite)]
        public GasMixture Air { get; private set; } = new();

        [DataField("port"), ViewVariables(VVAccess.ReadWrite)]
        public string PortName { get; set; } = "port";

        /// <summary>
        /// Which gases this machine will scrub out.
        /// Unlike fixed scrubbers controlled by an air alarm,
        /// this can't be changed in game.
        /// </summary>
        [DataField("filterGases")]
        public HashSet<Gas> FilterGases = new()
        {
            Gas.CarbonDioxide,
            Gas.Plasma,
            Gas.Tritium,
            Gas.WaterVapor,
            Gas.Ammonia,
            Gas.NitrousOxide,
            Gas.Frezon,
            Gas.BZ, // Funky atmos - /tg/ gases
            Gas.Healium, // Funky atmos - /tg/ gases
            Gas.Nitrium, // Funky atmos - /tg/ gases
            Gas.Hydrogen, // Funky atmos - /tg/ gases
            Gas.HyperNoblium, // Funky atmos - /tg/ gases
            Gas.ProtoNitrate, // Funky atmos - /tg/ gases
            Gas.Zauker, // Funky atmos - /tg/ gases
            Gas.Halon, // Funky atmos - /tg/ gases
            Gas.Helium, // Funky atmos - /tg/ gases
            Gas.AntiNoblium, // Funky atmos - /tg/ gases
        };

        [ViewVariables(VVAccess.ReadWrite)]
        public bool Enabled = true;

        /// <summary>
        /// Maximum internal pressure before it refuses to take more.
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public float MaxPressure = 2500;

        /// <summary>
        /// The speed at which gas is scrubbed from the environment.
        /// </summary>
        [DataField, ViewVariables(VVAccess.ReadWrite)]
        public float TransferRate = 800;

        #region GuidebookData

        [GuidebookData]
        public float Volume => Air.Volume;

        #endregion
    }
}
