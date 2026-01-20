// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2023 Jezithyr
// SPDX-FileCopyrightText: 2022 CrudeWax
// SPDX-License-Identifier: MIT

using Content.Shared.Mobs;

namespace Content.Client.DamageState;

[RegisterComponent]
public sealed partial class DamageStateVisualsComponent : Component
{
    public int? OriginalDrawDepth;

    [DataField("states")] public Dictionary<MobState, Dictionary<DamageStateVisualLayers, string>> States = new();
}

public enum DamageStateVisualLayers : byte
{
    Base,
    BaseUnshaded,
}
