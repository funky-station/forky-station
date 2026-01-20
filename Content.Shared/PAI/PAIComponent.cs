// SPDX-FileCopyrightText: 2025 PJB3005
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu
// SPDX-FileCopyrightText: 2025 Jessica M
// SPDX-FileCopyrightText: 2025 ArchRBX
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2024 mr-bo-jangles
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 20kdc
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
