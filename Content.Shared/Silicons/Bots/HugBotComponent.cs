// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Centronias <charlie.t.santos@gmail.com>
// SPDX-FileCopyrightText: 2026 taydeo <tay@funkystation.org>
// SPDX-FileCopyrightText: 2026 taydeo <td12233a@gmail.com>
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
