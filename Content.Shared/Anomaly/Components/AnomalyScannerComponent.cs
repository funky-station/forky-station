// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Quantum-cross <7065792+Quantum-cross@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 Velken <8467292+Velken@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Anomaly;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Anomaly.Components;

/// <summary>
/// This is used for scanning anomalies and
/// displaying information about them in the ui
/// </summary>
[RegisterComponent, Access(typeof(SharedAnomalyScannerSystem))]
[NetworkedComponent]
public sealed partial class AnomalyScannerComponent : Component
{
    /// <summary>
    /// The anomaly that was last scanned by this scanner.
    /// </summary>
    [ViewVariables]
    public EntityUid? ScannedAnomaly;

    /// <summary>
    /// How long the scan takes
    /// </summary>
    [DataField]
    public float ScanDoAfterDuration = 5;

    /// <summary>
    /// The sound plays when the scan finished
    /// </summary>
    [DataField]
    public SoundSpecifier? CompleteSound = new SoundPathSpecifier("/Audio/Items/beep.ogg");

    /// <summary>
    /// Whether to ignore the secret data on the anomaly.
    /// </summary>
    [DataField]
    public bool IgnoreSecret;
}
