// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Centronias
// SPDX-License-Identifier: MIT

namespace Content.Shared.Silicons.Bots;

/// <summary>
/// This component describes how a HugBot hugs.
/// </summary>
/// <see cref="SharedHugBotSystem"/>
[RegisterComponent, AutoGenerateComponentState]
public sealed partial class HugBotComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan HugCooldown = TimeSpan.FromMinutes(2);
}
