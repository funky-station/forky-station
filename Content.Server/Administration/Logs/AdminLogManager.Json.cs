// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2025 Leon Friedrich
// SPDX-FileCopyrightText: 2024 ShadowCommander
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 wrexbe
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Content.Server.Administration.Logs.Converters;
using Robust.Shared.Collections;

namespace Content.Server.Administration.Logs;

public sealed partial class AdminLogManager
{
    private static readonly JsonNamingPolicy NamingPolicy = JsonNamingPolicy.CamelCase;

    // Init only
    private JsonSerializerOptions _jsonOptions = default!;

    private void InitializeJson()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = NamingPolicy
        };

        var interfaces = new ValueList<IAdminLogConverter>();

        foreach (var converter in _reflection.FindTypesWithAttribute<AdminLogConverterAttribute>())
        {
            var instance = _typeFactory.CreateInstance<JsonConverter>(converter);
            if (instance is IAdminLogConverter converterInterface)
            {
                interfaces.Add(converterInterface);
                converterInterface.Init(_dependencies);
            }
            _jsonOptions.Converters.Add(instance);
        }

        foreach (var @interface in interfaces)
        {
            @interface.Init2(_jsonOptions);
        }

        var converterNames = _jsonOptions.Converters.Select(converter => converter.GetType().Name);
        _sawmill.Debug($"Admin log converters found: {string.Join(" ", converterNames)}");
    }
}
