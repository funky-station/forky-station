// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio.Systems;

namespace Content.Shared.Audio.Jukebox;

public abstract class SharedJukeboxSystem : EntitySystem
{
    [Dependency] protected readonly SharedAudioSystem Audio = default!;
}
