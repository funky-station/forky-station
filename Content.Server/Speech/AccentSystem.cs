// SPDX-FileCopyrightText: 2025 alexalexmax
// SPDX-FileCopyrightText: 2025 slarticodefast
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 TakoDragon
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using System.Text.RegularExpressions;
using Content.Shared.Chat;
using Content.Shared.Speech;

namespace Content.Server.Speech;

public sealed class AccentSystem : EntitySystem
{
    public static readonly Regex SentenceRegex = new(@"(?<=[\.!\?‽])(?![\.!\?‽])", RegexOptions.Compiled);

    public override void Initialize()
    {
        SubscribeLocalEvent<TransformSpeechEvent>(AccentHandler);
    }

    private void AccentHandler(TransformSpeechEvent args)
    {
        if (args.Cancelled)
            return;

        var accentEvent = new AccentGetEvent(args.Sender, args.Message);

        RaiseLocalEvent(args.Sender, accentEvent, true);
        args.Message = accentEvent.Message;
    }
}
