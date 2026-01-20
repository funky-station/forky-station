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

public sealed partial class KnockSpellEvent : InstantActionEvent
{
    /// <summary>
    /// The range this spell opens doors in
    /// 10f is the default
    /// Should be able to open all doors/lockers in visible sight
    /// </summary>
    [DataField]
    public float Range = 10f;
}
