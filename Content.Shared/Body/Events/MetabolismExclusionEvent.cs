// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Nikovnik <116634167+nkokic@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Chemistry.Reagent;

namespace Content.Shared.Body.Events;

/// <summary>
/// Event called by <see cref="Content.Server.Body.Systems.MetabolizerSystem"/> to get a list of
/// blood like reagents for metabolism to skip.
/// </summary>
[ByRefEvent]
public readonly record struct MetabolismExclusionEvent()
{
    public readonly List<ReagentId> Reagents = [];
}
