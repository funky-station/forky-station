// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-License-Identifier: MIT

using Content.Shared.Actions;
using Robust.Shared.Serialization.TypeSerializers.Implementations;

namespace Content.Shared.UserInterface;

public sealed partial class OpenUiActionEvent : InstantActionEvent
{
    [DataField(required: true, customTypeSerializer: typeof(EnumSerializer))]
    public Enum? Key { get; private set; }
}
