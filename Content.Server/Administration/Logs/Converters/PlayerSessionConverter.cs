// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2025 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using System.Text.Json;
using Content.Shared.Administration.Logs;

namespace Content.Server.Administration.Logs.Converters;

[AdminLogConverter]
public sealed class PlayerSessionConverter : AdminLogConverter<SerializablePlayer>
{
    public override void Write(Utf8JsonWriter writer, SerializablePlayer value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Uid is {Valid: true} playerEntity)
        {
            writer.WriteNumber("id", playerEntity.Id);
            writer.WriteString("name", value.Name);
        }

        writer.WriteString("player", value.UserId);

        writer.WriteEndObject();
    }
}
