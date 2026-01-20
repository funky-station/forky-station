// SPDX-FileCopyrightText: 2024 deltanedas
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Rainfey
// SPDX-FileCopyrightText: 2023 Ed
// SPDX-License-Identifier: MIT

using Content.Shared.Random;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Stores data for <see cref="ThiefRuleSystem"/>.
/// </summary>
[RegisterComponent, Access(typeof(ThiefRuleSystem))]
public sealed partial class ThiefRuleComponent : Component;
