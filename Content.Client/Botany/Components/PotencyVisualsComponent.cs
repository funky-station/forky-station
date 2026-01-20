// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 Steven K
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-License-Identifier: MIT

namespace Content.Client.Botany.Components;

[RegisterComponent]
public sealed partial class PotencyVisualsComponent : Component
{
    [DataField("minimumScale")]
    public float MinimumScale = 1f;

    [DataField("maximumScale")]
    public float MaximumScale = 2f;
}
