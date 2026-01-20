// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Moony
// SPDX-FileCopyrightText: 2022 Moony
// SPDX-License-Identifier: MIT

namespace Content.Server.Maps.NameGenerators;

[ImplicitDataDefinitionForInheritors]
public abstract partial class StationNameGenerator
{
    public abstract string FormatName(string input);
}
