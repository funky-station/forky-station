// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Body.Systems;

namespace Content.Shared.Body.Components;

[RegisterComponent, Access(typeof(BrainSystem))]
public sealed partial class BrainComponent : Component;
