// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Princess Cheeseballs <66055347+Princess-Cheeseballs@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert;

[Serializable, NetSerializable]
public record struct AlertState
{
    public short? Severity;
    public (TimeSpan startTime, TimeSpan endTime)? Cooldown;
    public bool AutoRemove;
    public bool ShowCooldown;
    public ProtoId<AlertPrototype> Type;
}
