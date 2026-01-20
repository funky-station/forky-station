// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Flipp Syder
// SPDX-License-Identifier: MIT

using Content.Shared.Preferences;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid.Prototypes;

[Prototype]
public sealed partial class HumanoidProfilePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("customBaseLayers")]
    public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers = new();

    [DataField("profile")]
    public HumanoidCharacterProfile Profile { get; private set; } = new();
}
