// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

namespace Content.Shared.AlertLevel;

[RegisterComponent]
public sealed partial class AlertLevelDisplayComponent : Component
{
    [DataField("alertVisuals")]
    public  Dictionary<string, string> AlertVisuals = new();
}
