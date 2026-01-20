// SPDX-FileCopyrightText: 2023 Moony
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;
using Robust.Shared.Console;
using Robust.Shared.Toolshed;

namespace Content.Shared.Administration
{
    /// <summary>
    ///     Specifies that a command can be executed by any player.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    [MeansImplicitUse]
    public sealed class AnyCommandAttribute : Attribute
    {

    }
}
