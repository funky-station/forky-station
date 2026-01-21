// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Temperature.Systems;

namespace Content.Client.Temperature.Systems;

/// <summary>
/// This exists so <see cref="SharedTemperatureSystem"/> runs on client/>
/// </summary>
public sealed class TemperatureSystem : SharedTemperatureSystem;
