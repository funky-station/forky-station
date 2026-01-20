// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Eui
{
    [Serializable]
    public abstract class EuiMessageBase
    {

    }

    [Serializable, NetSerializable]
    public sealed class CloseEuiMessage : EuiMessageBase
    {
    }
}
