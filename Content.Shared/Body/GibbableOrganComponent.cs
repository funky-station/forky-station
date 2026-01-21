// SPDX-FileCopyrightText: 2026 pathetic meowmeow <uhhadd@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Body;

[RegisterComponent, NetworkedComponent]
[Access(typeof(GibbableOrganSystem))]
public sealed partial class GibbableOrganComponent : Component;
