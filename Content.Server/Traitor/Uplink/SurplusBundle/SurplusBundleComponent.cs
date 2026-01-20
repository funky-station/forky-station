// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-License-Identifier: MIT

namespace Content.Server.Traitor.Uplink.SurplusBundle;

/// <summary>
///     Fill crate with a random uplink items.
/// </summary>
[RegisterComponent]
public sealed partial class SurplusBundleComponent : Component
{
    /// <summary>
    ///     Total price of all content inside bundle.
    /// </summary>
    [DataField]
    public int TotalPrice = 20;
}
