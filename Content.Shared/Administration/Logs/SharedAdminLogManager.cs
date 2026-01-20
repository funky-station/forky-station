// SPDX-FileCopyrightText: 2025 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-License-Identifier: MIT

using Content.Shared.Database;

namespace Content.Shared.Administration.Logs;

[Virtual]
public class SharedAdminLogManager : ISharedAdminLogManager
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public IEntityManager EntityManager => _entityManager;

    public bool Enabled { get; protected set; }

    public virtual string ConvertName(string name) => name;

    public virtual void Add(LogType type, LogImpact impact, ref LogStringHandler handler)
    {
        // noop
    }

    public virtual void Add(LogType type, ref LogStringHandler handler)
    {
        // noop
    }
}
