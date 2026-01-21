// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Centronias <charlie.t.santos@gmail.com>
// SPDX-License-Identifier: MIT

using Content.Shared.Silicons.Bots;

namespace Content.Server.Silicons.Bots;

/// <summary>
/// This marker component indicates that its entity has been recently hugged by a HugBot and should not be hugged again
/// before <see cref="CooldownCompleteAfter">a cooldown period</see> in order to prevent hug spam.
/// </summary>
/// <see cref="SharedHugBotSystem"/>
[RegisterComponent, AutoGenerateComponentPause]
public sealed partial class RecentlyHuggedByHugBotComponent : Component
{
    [DataField, AutoPausedField]
    public TimeSpan CooldownCompleteAfter = TimeSpan.MinValue;
}
