// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 ArtisticRoomba
// SPDX-FileCopyrightText: 2025 Partmedia
// SPDX-FileCopyrightText: 2024 eoineoineoin
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Slava0135
// SPDX-FileCopyrightText: 2023 James Simonson
// SPDX-FileCopyrightText: 2022 rolfero
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Paul Ritter
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 collinlunn
// SPDX-FileCopyrightText: 2020 ike709
// SPDX-FileCopyrightText: 2020 AJCM-git
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 py01
// SPDX-License-Identifier: MIT

using Content.Server.Power.NodeGroups;
using Content.Shared.APC;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Power.Components;

[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class ApcComponent : BaseApcNetComponent
{
    [DataField("onReceiveMessageSound")]
    public SoundSpecifier OnReceiveMessageSound = new SoundPathSpecifier("/Audio/Machines/machine_switch.ogg");

    public ApcChargeState LastChargeState;
    public TimeSpan? LastChargeStateTime;

    public ApcExternalPowerState LastExternalState;

    /// <summary>
    /// Time the ui was last updated automatically.
    /// Done after every <see cref="VisualsChangeDelay"/> to show the latest load.
    /// If charge state changes it will be instantly updated.
    /// </summary>
    public TimeSpan LastUiUpdate;

    [DataField("enabled")]
    public bool MainBreakerEnabled = true;

    /// <summary>
    /// APC state needs to always be updated after first processing tick.
    /// </summary>
    public bool NeedStateUpdate;

    public const float HighPowerThreshold = 0.9f;
    public static TimeSpan VisualsChangeDelay = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Maximum continuous load in Watts that this APC can supply to loads. Exceeding this starts a
    /// timer, which after enough overloading causes the APC to "trip" off.
    /// </summary>
    [DataField]
    public float MaxLoad = 20e3f;

    /// <summary>
    /// Time that the APC can be continuously overloaded before tripping off.
    /// </summary>
    [DataField]
    public TimeSpan TripTime = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Time that overloading began.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField]
    public TimeSpan? TripStartTime;

    /// <summary>
    /// Set to true if the APC tripped off. Used to indicate problems in the UI. Reset by switching
    /// APC on.
    /// </summary>
    [DataField]
    public bool TripFlag;

    // TODO ECS power a little better!
    // End the suffering
    protected override void AddSelfToNet(IApcNet apcNet)
    {
        apcNet.AddApc(Owner, this);
    }

    protected override void RemoveSelfFromNet(IApcNet apcNet)
    {
        apcNet.RemoveApc(Owner, this);
    }
}
