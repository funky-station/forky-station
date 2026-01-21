// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Network;

namespace Content.Shared.Administration.Logs;

/// <summary>
/// Interface implemented by admin log values that contain player references.
/// </summary>
public interface IAdminLogsPlayerValue
{
    IEnumerable<NetUserId> Players { get; }
}
