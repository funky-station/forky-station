// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

namespace Content.Shared.Emoting;

public sealed class EmoteSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EmoteAttemptEvent>(OnEmoteAttempt);
    }

    public void SetEmoting(EntityUid uid, bool value, EmotingComponent? component = null)
    {
        if (value && !Resolve(uid, ref component))
            return;

        component = EnsureComp<EmotingComponent>(uid);

        if (component.Enabled == value)
            return;

        Dirty(uid, component);
    }

    private void OnEmoteAttempt(EmoteAttemptEvent args)
    {
        if (!TryComp(args.Uid, out EmotingComponent? emote) || !emote.Enabled)
            args.Cancel();
    }
}
