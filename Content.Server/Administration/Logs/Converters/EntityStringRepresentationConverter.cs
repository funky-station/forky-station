// SPDX-FileCopyrightText: 2025 Leon Friedrich
// SPDX-FileCopyrightText: 2024 Chief-Engineer
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using System.Text.Json;
using Content.Server.Administration.Managers;

namespace Content.Server.Administration.Logs.Converters;

[AdminLogConverter]
public sealed class EntityStringRepresentationConverter : AdminLogConverter<EntityStringRepresentation>
{
    [Dependency] private readonly IAdminManager _adminManager = default!;

    public override void Write(Utf8JsonWriter writer, EntityStringRepresentation value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("id", (int) value.Uid);

        if (value.Name != null)
        {
            writer.WriteString("name", value.Name);
        }

        if (value.Session != null)
        {
            writer.WriteString("player", value.Session.UserId.UserId);

            if (_adminManager.IsAdmin(value.Session))
            {
                writer.WriteBoolean("admin", true);
            }
        }

        if (value.Prototype != null)
        {
            writer.WriteString("prototype", value.Prototype);
        }

        if (value.Deleted)
        {
            writer.WriteBoolean("deleted", true);
        }

        writer.WriteEndObject();
    }
}
