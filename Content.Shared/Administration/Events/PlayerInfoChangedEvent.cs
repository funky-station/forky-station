// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events;

[NetSerializable, Serializable]
public sealed class PlayerInfoChangedEvent : EntityEventArgs
{
    public PlayerInfo? PlayerInfo;
}
