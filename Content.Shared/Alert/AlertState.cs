// SPDX-FileCopyrightText: 2025 Princess Cheeseballs
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Acruid
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
