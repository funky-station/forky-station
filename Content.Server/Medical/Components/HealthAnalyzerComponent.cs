// SPDX-FileCopyrightText: 2025 Minemoder5000
// SPDX-FileCopyrightText: 2024 ArchRBX
// SPDX-FileCopyrightText: 2024 Milon
// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Fildrance
// SPDX-FileCopyrightText: 2024 Rainfey
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2023 nmajask
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 Fishfish458
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.Medical.Components;

/// <summary>
/// After scanning, retrieves the target Uid to use with its related UI.
/// </summary>
/// <remarks>
/// Requires <c>ItemToggleComponent</c>.
/// </remarks>
[RegisterComponent, AutoGenerateComponentPause]
[Access(typeof(HealthAnalyzerSystem), typeof(CryoPodSystem))]
public sealed partial class HealthAnalyzerComponent : Component
{
    /// <summary>
    /// When should the next update be sent for the patient
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoPausedField]
    public TimeSpan NextUpdate = TimeSpan.Zero;

    /// <summary>
    /// The delay between patient health updates
    /// </summary>
    [DataField]
    public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

    /// <summary>
    /// How long it takes to scan someone.
    /// </summary>
    [DataField]
    public TimeSpan ScanDelay = TimeSpan.FromSeconds(0.8);

    /// <summary>
    /// Which entity has been scanned, for continuous updates
    /// </summary>
    [DataField]
    public EntityUid? ScannedEntity;

    /// <summary>
    /// The maximum range in tiles at which the analyzer can receive continuous updates, a value of null will be infinite range
    /// </summary>
    [DataField]
    public float? MaxScanRange = 2.5f;

    /// <summary>
    /// Sound played on scanning begin
    /// </summary>
    [DataField]
    public SoundSpecifier? ScanningBeginSound;

    /// <summary>
    /// Sound played on scanning end
    /// </summary>
    [DataField]
    public SoundSpecifier ScanningEndSound = new SoundPathSpecifier("/Audio/Items/Medical/healthscanner.ogg");

    /// <summary>
    /// Whether to show up the popup
    /// </summary>
    [DataField]
    public bool Silent;
}
