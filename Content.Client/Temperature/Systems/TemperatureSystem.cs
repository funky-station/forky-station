// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-License-Identifier: MIT

using Content.Shared.Temperature.Systems;

namespace Content.Client.Temperature.Systems;

/// <summary>
/// This exists so <see cref="SharedTemperatureSystem"/> runs on client/>
/// </summary>
public sealed class TemperatureSystem : SharedTemperatureSystem;
