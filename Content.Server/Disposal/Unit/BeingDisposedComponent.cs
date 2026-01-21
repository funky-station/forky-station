// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-License-Identifier: MIT

namespace Content.Server.Disposal.Unit;

/// <summary>
///     A component added to entities that are currently in disposals.
/// </summary>
[RegisterComponent]
public sealed partial class BeingDisposedComponent : Component
{
    [ViewVariables]
    public EntityUid Holder;
}
