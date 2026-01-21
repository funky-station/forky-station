// SPDX-FileCopyrightText: 2025 J <billsmith116@gmail.com>
// SPDX-FileCopyrightText: 2025 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Magic.Events;

public sealed partial class VoidApplauseSpellEvent : EntityTargetActionEvent
{
    /// <summary>
    ///     Emote to use.
    /// </summary>
    [DataField]
    public ProtoId<EmotePrototype> Emote = "ClapSingle";

    /// <summary>
    ///     Visual effect entity that is spawned at both the user's and the target's location.
    /// </summary>
    [DataField]
    public EntProtoId Effect = "EffectVoidBlink";
}
