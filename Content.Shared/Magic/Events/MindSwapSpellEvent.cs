// SPDX-FileCopyrightText: 2024 ActiveMammmoth <140334666+ActiveMammmoth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 J <billsmith116@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;

namespace Content.Shared.Magic.Events;

public sealed partial class MindSwapSpellEvent : EntityTargetActionEvent
{
    [DataField]
    public TimeSpan PerformerStunDuration = TimeSpan.FromSeconds(10);

    [DataField]
    public TimeSpan TargetStunDuration = TimeSpan.FromSeconds(10);
}
