// SPDX-FileCopyrightText: 2026 ScarKy0
// SPDX-FileCopyrightText: 2024 Verm
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 08A
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Swept
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Memory
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-License-Identifier: MIT

using Content.Shared.Tools.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Tools.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedToolSystem))]
public sealed partial class ToolComponent : Component
{
    [DataField]
    public PrototypeFlags<ToolQualityPrototype> Qualities  = [];

    /// <summary>
    ///     For tool interactions that have a delay before action this will modify the rate, time to wait is divided by this value
    /// </summary>
    [DataField]
    public float SpeedModifier = 1f;

    [DataField]
    public SoundSpecifier? UseSound;
}

/// <summary>
/// Attempt event called *before* any do afters to see if the tool usage should succeed or not.
/// Raised on both the tool and then target.
/// </summary>
public sealed class ToolUseAttemptEvent(EntityUid user, float fuel) : CancellableEntityEventArgs
{
    public EntityUid User { get; } = user;
    public float Fuel = fuel;
}

/// <summary>
/// Event raised on the user of a tool to see if they can actually use it.
/// </summary>
[ByRefEvent]
public struct ToolUserAttemptUseEvent(EntityUid? target)
{
    public EntityUid? Target = target;
    public bool Cancelled = false;
}
