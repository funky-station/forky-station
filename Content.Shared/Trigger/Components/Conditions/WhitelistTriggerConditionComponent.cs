// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Conditions;

/// <summary>
/// Checks if the user of a trigger satisfies a whitelist and blacklist condition.
/// Cancels the trigger otherwise.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WhitelistTriggerConditionComponent : BaseTriggerConditionComponent
{
    /// <summary>
    /// Whitelist for what entities can cause this trigger.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? UserWhitelist;

    /// <summary>
    /// Blacklist for what entities can cause this trigger.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? UserBlacklist;
}
