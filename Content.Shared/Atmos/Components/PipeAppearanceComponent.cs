// SPDX-FileCopyrightText: 2025 chromiumboy
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-License-Identifier: MIT

using Robust.Shared.Utility;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
public sealed partial class PipeAppearanceComponent : Component
{
    [DataField]
    public SpriteSpecifier.Rsi[] Sprite = [new(new("Structures/Piping/Atmospherics/pipe.rsi"), "pipeConnector"),
        new(new("Structures/Piping/Atmospherics/pipe_alt1.rsi"), "pipeConnector"),
        new(new("Structures/Piping/Atmospherics/pipe_alt2.rsi"), "pipeConnector")];
}
