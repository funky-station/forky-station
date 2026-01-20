// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Morb
// SPDX-License-Identifier: MIT

using Content.Shared.Chat;
using Content.Server.Chat.Systems;
using Robust.Shared.Prototypes;

namespace Content.Server.Speech;

public sealed partial class EmotesMenuSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeAllEvent<PlayEmoteMessage>(OnPlayEmote);
    }

    private void OnPlayEmote(PlayEmoteMessage msg, EntitySessionEventArgs args)
    {
        var player = args.SenderSession.AttachedEntity;
        if (!player.HasValue)
            return;

        if (!_prototypeManager.Resolve(msg.ProtoId, out var proto) || proto.ChatTriggers.Count == 0)
            return;

        _chat.TryEmoteWithChat(player.Value, msg.ProtoId);
    }
}
