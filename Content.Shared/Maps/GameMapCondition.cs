// SPDX-FileCopyrightText: 2025 Errant
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-License-Identifier: MIT

namespace Content.Shared.Maps;

[ImplicitDataDefinitionForInheritors]
public abstract partial class GameMapCondition
{
    [DataField("inverted")]
    public bool Inverted { get; private set; }
    public abstract bool Check(GameMapPrototype map);
}
