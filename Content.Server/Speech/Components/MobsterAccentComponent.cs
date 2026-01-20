// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-License-Identifier: MIT

namespace Content.Server.Speech.Components;

/// <summary>
///     Nyehh, my gabagool, see?
///     Etc etc.
/// </summary>
[RegisterComponent]
public sealed partial class MobsterAccentComponent : Component
{
    /// <summary>
    ///     Do you make all the rules?
    /// </summary>
    [DataField("isBoss")]
    public bool IsBoss = true;
}
