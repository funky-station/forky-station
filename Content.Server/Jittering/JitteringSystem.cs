// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Jittering;

namespace Content.Server.Jittering
{
    public sealed class JitteringSystem : SharedJitteringSystem
    {
        // This entity system only exists on the server so it will be registered, otherwise we can't use SharedJitteringSystem...
    }
}
