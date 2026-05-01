using System.Collections.Generic;
using Robust.Shared.Network;

namespace Content.Shared.Administration.Logs;

/// <summary>
/// Attributes an admin log line to a player when no <see cref="SerializablePlayer"/> session is available.
/// </summary>
public readonly record struct AdminLogAttributedUser(NetUserId User) : IAdminLogsPlayerValue
{
    IEnumerable<NetUserId> IAdminLogsPlayerValue.Players
    {
        get { yield return User; }
    }
}
