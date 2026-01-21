// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Polymorph;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Trigger.Components.Effects;

/// <summary>
/// Polymorphs the enity when triggered.
/// If TargetUser is true it will polymorph the user instead.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PolymorphOnTriggerComponent : BaseXOnTriggerComponent
{
    /// <summary>
    /// Polymorph settings.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<PolymorphPrototype> Polymorph;
}
