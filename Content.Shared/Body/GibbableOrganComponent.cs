// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2026 pathetic meowmeow <uhhadd@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Body;

[RegisterComponent, NetworkedComponent]
[Access(typeof(GibbableOrganSystem))]
public sealed partial class GibbableOrganComponent : Component;
