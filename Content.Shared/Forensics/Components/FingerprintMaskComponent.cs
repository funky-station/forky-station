// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.Forensics.Components;

/// <summary>
/// This component stops the entity from leaving fingerprints,
/// usually so fibres can be left instead.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FingerprintMaskComponent : Component;

