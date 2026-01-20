// SPDX-FileCopyrightText: 2025 pathetic meowmeow
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2022 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Galactic Chimp
// SPDX-FileCopyrightText: 2020 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Construction.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class MachineBoardComponent : Component
{
    /// <summary>
    /// The stacks needed to construct this machine
    /// </summary>
    [DataField]
    public Dictionary<ProtoId<StackPrototype>, int> StackRequirements = new();

    /// <summary>
    /// Entities needed to construct this machine, discriminated by tag.
    /// </summary>
    [DataField]
    public Dictionary<ProtoId<TagPrototype>, GenericPartInfo> TagRequirements = new();

    /// <summary>
    /// Entities needed to construct this machine, discriminated by component.
    /// </summary>
    [DataField]
    public Dictionary<string, GenericPartInfo> ComponentRequirements = new();

    /// <summary>
    /// The machine that's constructed when this machine board is completed.
    /// </summary>
    [DataField(required: true)]
    public EntProtoId Prototype;
}

/// <summary>
/// Marker component for any item that's machine board-like without necessarily being a MachineBoardComponent
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class CircuitboardComponent : Component;

[DataDefinition, Serializable]
public partial struct GenericPartInfo
{
    [DataField(required: true)]
    public int Amount;

    [DataField(required: true)]
    public EntProtoId DefaultPrototype;

    [DataField]
    public LocId? ExamineName;
}
