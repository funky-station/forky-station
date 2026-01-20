// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;

namespace Content.Server.Administration.Logs.Converters;

[AttributeUsage(AttributeTargets.Class)]
[BaseTypeRequired(typeof(AdminLogConverter<>))]
[MeansImplicitUse]
public sealed class AdminLogConverterAttribute : Attribute
{
}
