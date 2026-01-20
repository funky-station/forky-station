// SPDX-FileCopyrightText: 2025 nikitosych
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Guardian;

[Serializable, NetSerializable]
public sealed partial class GuardianCreatorDoAfterEvent : SimpleDoAfterEvent
{
}
