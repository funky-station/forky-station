// SPDX-FileCopyrightText: 2024 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Ignaz "Ian" Kraft <ignaz.k@live.de>
// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Reagent;

[ImplicitDataDefinitionForInheritors, Serializable, NetSerializable]
public sealed partial class DnaData : ReagentData
{
    [DataField]
    public string DNA = string.Empty;

    public override ReagentData Clone()
    {
        return new DnaData
        {
            DNA = DNA,
        };
    }

    public override bool Equals(ReagentData? other)
    {
        if (other == null)
        {
            return false;
        }

        return ((DnaData) other).DNA == DNA;
    }

    public override int GetHashCode()
    {
        return DNA.GetHashCode();
    }
}
