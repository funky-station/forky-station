// SPDX-FileCopyrightText: 2024 Simon <63975668+Simyon264@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System.Threading.Tasks;
using Content.Server.Database;
using Robust.Shared.Network;

namespace Content.Server.Connection.Whitelist.Conditions;

/// <summary>
/// Condition that matches if the player is in the manual blacklist.
/// </summary>
public sealed partial class ConditionManualBlacklistMembership : WhitelistCondition
{
}
