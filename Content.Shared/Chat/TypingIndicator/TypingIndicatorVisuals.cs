// SPDX-FileCopyrightText: 2025 lzk
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Chat.TypingIndicator;

[Serializable, NetSerializable]
public enum TypingIndicatorVisuals : byte
{
    State
}

[Serializable]
public enum TypingIndicatorLayers : byte
{
    Base
}
