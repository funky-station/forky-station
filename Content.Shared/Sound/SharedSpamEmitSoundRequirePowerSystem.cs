// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-License-Identifier: MIT

namespace Content.Shared.Sound;

public abstract partial class SharedSpamEmitSoundRequirePowerSystem : EntitySystem
{
    [Dependency] protected readonly SharedEmitSoundSystem EmitSound = default!;
}
