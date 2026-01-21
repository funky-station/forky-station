// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests.Helpers;

/// <summary>
/// Component that is used by <see cref="TestListenerSystem{TEvent}"/> to store any information about received events.
/// </summary>
[RegisterComponent]
public sealed partial class TestListenerComponent : Component
{
    public Dictionary<Type, List<object>> Events = new();
}
