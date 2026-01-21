// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System.Runtime.CompilerServices;
using Content.Shared.Database;

namespace Content.Shared.Administration.Logs;

public interface ISharedAdminLogManager
{
    public bool Enabled { get; }

    // JsonNamingPolicy is not whitelisted by the sandbox.
    public string ConvertName(string name);

    // Required for the log string interpolation handler to access ToPrettyString()
    public IEntityManager EntityManager { get; }

    void Add(LogType type, LogImpact impact, [InterpolatedStringHandlerArgument("")] ref LogStringHandler handler);

    void Add(LogType type, [InterpolatedStringHandlerArgument("")] ref LogStringHandler handler);
}
