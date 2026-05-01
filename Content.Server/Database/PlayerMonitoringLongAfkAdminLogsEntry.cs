// SPDX-FileCopyrightText: 2026 Space Wizards Federation
// SPDX-License-Identifier: MIT

namespace Content.Server.Database;

/// <summary>
/// Result of scanning admin logs for long idle gaps at round end.
/// </summary>
public sealed record PlayerMonitoringLongAfkAdminLogsEntry(Guid UserId, string LastSeenUserName, double MaxIdleMinutes);
