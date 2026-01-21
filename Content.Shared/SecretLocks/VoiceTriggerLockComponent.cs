// SPDX-FileCopyrightText: 2025 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.SecretLocks;

/// <summary>
/// "Locks" items (Doesn't actually lock them but just switches various settings) so its not possible to tell
/// the item is triggered by a voice activation.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class VoiceTriggerLockComponent : Component;
