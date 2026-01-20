// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Bixkitts
// SPDX-License-Identifier: MIT

namespace Content.Client.Smoking;

[RegisterComponent]
public sealed partial class BurnStateVisualsComponent : Component
{
    [DataField("burntIcon")]
    public string BurntIcon = "burnt-icon";
    [DataField("litIcon")]
    public string LitIcon = "lit-icon";
    [DataField("unlitIcon")]
    public string UnlitIcon = "icon";
}

