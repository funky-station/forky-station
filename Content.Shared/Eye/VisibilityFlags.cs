// SPDX-FileCopyrightText: 2023, 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Eye
{
    [Flags]
    [FlagsFor(typeof(VisibilityMaskLayer))]
    public enum VisibilityFlags : int
    {
        None = 0,
        Normal = 1 << 0,
        Ghost = 1 << 1, // Observers and revenants.
        Subfloor = 1 << 2, // Pipes, disposal chutes, cables etc. while hidden under tiles. Can be revealed with a t-ray.
        Admin = 1 << 3, // Reserved for admins in stealth mode and admin tools.
    }
}
