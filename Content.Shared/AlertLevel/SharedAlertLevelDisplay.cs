// SPDX-FileCopyrightText: 2023 Menshin
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.AlertLevel;

[Serializable, NetSerializable]
public enum AlertLevelDisplay
{
    CurrentLevel,
    Layer,
    Powered
}
