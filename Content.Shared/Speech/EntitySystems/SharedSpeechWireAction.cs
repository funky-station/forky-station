// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Speech;

[Serializable, NetSerializable]
public enum SpeechWireActionKey : byte
{
    StatusKey,
}
