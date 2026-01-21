// SPDX-FileCopyrightText: 2021 20kdc <asdd2808@gmail.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 mr-bo-jangles <mr-bo-jangles@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 Jessica M <jessica@jessicamaybe.com>
// SPDX-FileCopyrightText: 2025 ArchRBX <5040911+ArchRBX@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared.PAI;

/// <summary>
/// pAIs, or Personal AIs, are essentially portable ghost role generators.
/// In their current implementation in SS14, they create a ghost role anyone can access,
/// and that a player can also "wipe" (reset/kick out player).
/// Theoretically speaking pAIs are supposed to use a dedicated "offer and select" system,
///  with the player holding the pAI being able to choose one of the ghosts in the round.
/// This seems too complicated for an initial implementation, though,
///  and there's not always enough players and ghost roles to justify it.
/// All logic in PAISystem.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PAIComponent : Component
{
    /// <summary>
    /// The last person who activated this PAI.
    /// Used for assigning the name.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntityUid? LastUser;

    /// <summary>
    /// When microwaved there is this chance to brick the pai, kicking out its player and preventing it from being used again.
    /// </summary>
    [DataField]
    public float BrickChance = 0.5f;

    /// <summary>
    /// Locale id for the popup shown when the pai gets bricked.
    /// </summary>
    [DataField]
    public string BrickPopup = "pai-system-brick-popup";

    /// <summary>
    /// Locale id for the popup shown when the pai is microwaved but does not get bricked.
    /// </summary>
    [DataField]
    public string ScramblePopup = "pai-system-scramble-popup";
}
