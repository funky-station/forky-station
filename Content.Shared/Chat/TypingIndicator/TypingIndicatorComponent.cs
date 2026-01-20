// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-FileCopyrightText: 2024 beck-thompson
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Morb
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Chat.TypingIndicator;

/// <summary>
///     Show typing indicator icon when player typing text in chat box.
///     Added automatically when player poses entity.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedTypingIndicatorSystem))]
public sealed partial class TypingIndicatorComponent : Component
{
    /// <summary>
    ///     Prototype id that store all visual info about typing indicator.
    /// </summary>
    [DataField("proto"), AutoNetworkedField]
    public ProtoId<TypingIndicatorPrototype> TypingIndicatorPrototype = "default";
}
