// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Explosion.Components;

namespace Content.Server.Xenoarchaeology.Artifact.XAE.Components;

/// <summary>
/// Activates <see cref="ExplosiveComponent"/> to explode.
/// </summary>
[RegisterComponent, Access(typeof(XAETriggerExplosivesSystem))]
public sealed partial class XAETriggerExplosivesComponent : Component;
