// SPDX-FileCopyrightText: 2023 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 imatsoup <93290208+imatsoup@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Content.Server.Thief.Systems;
using Content.Shared.Thief;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Thief.Components;

/// <summary>
/// This component stores the possible contents of the backpack,
/// which can be selected via the interface.
/// </summary>
[RegisterComponent, Access(typeof(ThiefUndeterminedBackpackSystem))]
public sealed partial class ThiefUndeterminedBackpackComponent : Component
{
    /// <summary>
    /// List of sets available for selection
    /// </summary>
    [DataField]
    public List<ProtoId<ThiefBackpackSetPrototype>> PossibleSets = new();

    [DataField]
    public List<int> SelectedSets = new();

    [DataField]
    public SoundCollectionSpecifier ApproveSound = new SoundCollectionSpecifier("storageRustle");

    /// <summary>
    /// Max number of sets you can select.
    /// </summary>
    [DataField]
    public int MaxSelectedSets = 2;

    /// <summary>
    /// Title field for undetermined equipment ui.
    /// </summary>
    [DataField]
    public LocId ToolName = "thief-backpack-window-title";

    /// <summary>
    /// Description field for undetermined equipment ui.
    /// </summary>
    [DataField]
    public LocId ToolDesc = "thief-backpack-window-description";

    /// <summary>
    /// What entity all the spawned items will appear inside of
    /// If null, will instead drop on the ground.
    /// </summary>
    [DataField]
    public EntProtoId? SpawnedStoragePrototype;
}
