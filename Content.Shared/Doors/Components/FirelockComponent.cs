// SPDX-FileCopyrightText: 2025 ShadowCommander
// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2024 nikthechampiongr
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Tom Leys
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 vulppine
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 tmtmtl30
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Git-Nivrak
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-License-Identifier: MIT

using Content.Shared.Guidebook;
using Robust.Shared.GameStates;

namespace Content.Shared.Doors.Components
{
    /// <summary>
    /// Companion component to <see cref="DoorComponent"/> that handles firelock-specific behavior, including
    /// auto-closing on depressurization, air/fire alarm interactions, and preventing normal door functions when
    /// retaining pressure..
    /// </summary>
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class FirelockComponent : Component
    {
        #region Settings

        /// <summary>
        /// Pry time modifier to be used when the firelock is currently closed due to fire or pressure.
        /// </summary>
        /// <returns></returns>
        [DataField("lockedPryTimeModifier"), ViewVariables(VVAccess.ReadWrite)]
        public float LockedPryTimeModifier = 1.5f;

        /// <summary>
        /// Maximum pressure difference before the firelock will refuse to open, in kPa.
        /// </summary>
        [DataField("pressureThreshold"), ViewVariables(VVAccess.ReadWrite)]
        [GuidebookData]
        public float PressureThreshold = 20;

        /// <summary>
        /// Maximum temperature difference before the firelock will refuse to open, in k.
        /// </summary>
        [DataField("temperatureThreshold"), ViewVariables(VVAccess.ReadWrite)]
        [GuidebookData]
        public float TemperatureThreshold = 330;
        // this used to check for hot-spots, but because accessing that data is a a mess this now just checks
        // temperature. This does mean a cold room will trigger hot-air pop-ups

        /// <summary>
        /// If true, and if this door has an <see cref="AtmosAlarmableComponent"/>, then it will only auto-close if the
        /// alarm is set to danger.
        /// </summary>
        [DataField("alarmAutoClose"), ViewVariables(VVAccess.ReadWrite)]
        public bool AlarmAutoClose = true;

        /// <summary>
        /// The cooldown duration before a firelock can automatically close due to a hazardous environment after it has
        /// been pried open. Measured in seconds.
        /// </summary>
        [DataField]
        public TimeSpan EmergencyCloseCooldownDuration = TimeSpan.FromSeconds(2);

        #endregion

        #region Set by system

        /// <summary>
        /// When the firelock will be allowed to automatically close again due to a hazardous environment.
        /// </summary>
        [DataField]
        public TimeSpan? EmergencyCloseCooldown;

        /// <summary>
        /// Whether the firelock can open, or is locked due to its environment.
        /// </summary>
        public bool IsLocked => Pressure || Temperature;

        /// <summary>
        /// Whether the firelock is holding back a hazardous pressure.
        /// </summary>
        [DataField, AutoNetworkedField]
        public bool Pressure;

        /// <summary>
        /// Whether the firelock is holding back extreme temperatures.
        /// </summary>
        [DataField, AutoNetworkedField]
        public bool Temperature;

        /// <summary>
        /// Whether the airlock is powered.
        /// </summary>
        [DataField, AutoNetworkedField]
        public bool Powered;

        #endregion

        #region Client animation

        /// <summary>
        /// The sprite state used to animate the airlock frame when the airlock opens.
        /// </summary>
        [DataField]
        public string OpeningLightSpriteState = "opening_unlit";

        /// <summary>
        /// The sprite state used to animate the airlock frame when the airlock closes.
        /// </summary>
        [DataField]
        public string ClosingLightSpriteState = "closing_unlit";

        /// <summary>
        /// The sprite state used to animate the airlock panel when the airlock opens.
        /// </summary>
        [DataField]
        public string OpeningPanelSpriteState = "panel_opening";

        /// <summary>
        /// The sprite state used to animate the airlock panel when the airlock closes.
        /// </summary>
        [DataField]
        public string ClosingPanelSpriteState = "panel_closing";

        /// <summary>
        /// The sprite state used for the open airlock lights.
        /// </summary>
        [DataField]
        public string OpenLightSpriteState = "open_unlit";

        /// <summary>
        /// The sprite state used for the closed airlock lights.
        /// </summary>
        [DataField]
        public string WarningLightSpriteState = "closed_unlit";

        /// <summary>
        /// The sprite state used for the 'access denied' lights animation.
        /// </summary>
        [DataField]
        public string DenySpriteState = "deny_unlit";

        #endregion
    }
}
