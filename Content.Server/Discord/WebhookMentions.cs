// SPDX-FileCopyrightText: 2023 LankLTE
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-License-Identifier: MIT

using System.Text.Json.Serialization;

namespace Content.Server.Discord;

public struct WebhookMentions
{
    [JsonPropertyName("parse")]
    public HashSet<string> Parse { get; set; } = new();

    public WebhookMentions()
    {
    }

    public void AllowRoleMentions()
    {
        Parse.Add("roles");
    }
}
