// SPDX-FileCopyrightText: 2025 BarryNorfolk <barrynorfolkman@protonmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

namespace Content.Client.Machines.Components;

/// <summary>
/// Component attached to all multipart machine ghosts
/// Intended for client side usage only, but used on prototypes.
/// </summary>
[RegisterComponent]
public sealed partial class MultipartMachineGhostComponent : Component
{
    /// <summary>
    /// Machine this particular ghost is linked to.
    /// </summary>
    public EntityUid? LinkedMachine = null;
}
