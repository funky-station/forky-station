// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 0x6273
// SPDX-FileCopyrightText: 2022 Julian Giebel
// SPDX-License-Identifier: MIT

namespace Content.Client.PDA;

/// <summary>
/// Used for specifying the pda windows border colors
/// </summary>
[RegisterComponent]
public sealed partial class PdaBorderColorComponent : Component
{
    [DataField("borderColor", required: true)]
    public string? BorderColor;


    [DataField("accentHColor")]
    public string? AccentHColor;


    [DataField("accentVColor")]
    public string? AccentVColor;
}
