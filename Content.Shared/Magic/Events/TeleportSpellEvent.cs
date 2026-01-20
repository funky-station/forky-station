// SPDX-FileCopyrightText: 2025 J
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2024 keronshb
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Jessica M
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;

namespace Content.Shared.Magic.Events;

// TODO: Can probably just be an entity or something
public sealed partial class TeleportSpellEvent : WorldTargetActionEvent
{

    // TODO: Move to magic component
    // TODO: Maybe not since sound specifier is a thing
    // Keep here to remind what the volume was set as
    /// <summary>
    /// Volume control for the spell.
    /// </summary>
    [DataField]
    public float BlinkVolume = 5f;
}
