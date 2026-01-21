// SPDX-FileCopyrightText: 2024 Saphire <lattice@saphi.re>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Singularity.EntitySystems;
using Content.Shared.Singularity.Components;

namespace Content.Client.Singularity.Systems;

/// <summary>
/// The client-side version of <see cref="SharedSingularityGeneratorSystem"/>.
/// Manages <see cref="SingularityGeneratorComponent"/>s.
/// Exists to make relevant signal handlers (ie: <see cref="SharedSingularityGeneratorSystem.OnEmagged"/>) work on the client.
/// </summary>
public sealed class SingularityGeneratorSystem : SharedSingularityGeneratorSystem
{}
