// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Access.Systems;

namespace Content.Shared.Access.Components;

[RegisterComponent, Access(typeof(IdExaminableSystem))]
public sealed partial class IdExaminableComponent : Component;
