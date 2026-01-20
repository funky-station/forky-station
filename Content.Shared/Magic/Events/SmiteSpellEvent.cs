// SPDX-FileCopyrightText: 2025 J
// SPDX-FileCopyrightText: 2024 keronshb
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;

namespace Content.Shared.Magic.Events;

public sealed partial class SmiteSpellEvent : EntityTargetActionEvent
{
    // TODO: Make part of gib method
    /// <summary>
    /// Should this smite delete all parts/mechanisms gibbed except for the brain?
    /// </summary>
    [DataField]
    public bool DeleteNonBrainParts = true;
}
