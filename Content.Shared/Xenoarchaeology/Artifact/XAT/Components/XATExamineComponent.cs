// SPDX-FileCopyrightText: 2025 Fildrance <fildrance@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT.Components;

/// <summary>
/// This is used for an artifact that is activated when someone examines it.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(XATExamineSystem))]
public sealed partial class XATExamineComponent : Component;
