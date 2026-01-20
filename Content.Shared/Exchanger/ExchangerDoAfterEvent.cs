// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Exchanger;

[Serializable, NetSerializable]
public sealed partial class ExchangerDoAfterEvent : SimpleDoAfterEvent
{
}
