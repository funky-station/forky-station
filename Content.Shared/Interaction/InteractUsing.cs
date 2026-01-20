// SPDX-FileCopyrightText: 2025 ScarKy0
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2021 Paul
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Julian Giebel
// SPDX-FileCopyrightText: 2020 juliangiebel
// SPDX-License-Identifier: MIT

using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Utility;

namespace Content.Shared.Interaction;

/// <summary>
///     Raised when a target entity is interacted with by a user while holding an object in their hand.
/// </summary>
[PublicAPI]
public sealed class InteractUsingEvent : HandledEntityEventArgs
{
    /// <summary>
    ///     Entity that triggered the interaction.
    /// </summary>
    public EntityUid User { get; }

    /// <summary>
    ///     Entity that the user used to interact.
    /// </summary>
    public EntityUid Used { get; }

    /// <summary>
    ///     Entity that was interacted on.
    /// </summary>
    public EntityUid Target { get; }

    /// <summary>
    ///     The original location that was clicked by the user.
    /// </summary>
    public EntityCoordinates ClickLocation { get; }

    public InteractUsingEvent(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation)
    {
        // Interact using should not have the same used and target.
        // That should be a use-in-hand event instead.
        // If this is not the case, can lead to bugs (e.g., attempting to merge a item stack into itself).
        DebugTools.Assert(used != target);

        User = user;
        Used = used;
        Target = target;
        ClickLocation = clickLocation;
    }
}

/// <summary>
/// Raised when a user entity interacts with a target while holding an object in their hand.
/// </summary>
[PublicAPI]
public sealed class UserInteractUsingEvent : HandledEntityEventArgs
{
    /// <summary>
    ///     Entity that triggered the interaction.
    /// </summary>
    public EntityUid User { get; }

    /// <summary>
    ///     Entity that the user used to interact.
    /// </summary>
    public EntityUid Used { get; }

    /// <summary>
    ///     Entity that was interacted on.
    /// </summary>
    public EntityUid Target { get; }

    /// <summary>
    ///     The original location that was clicked by the user.
    /// </summary>
    public EntityCoordinates ClickLocation { get; }

    public UserInteractUsingEvent(EntityUid user, EntityUid used, EntityUid target, EntityCoordinates clickLocation)
    {
        // Interact using should not have the same used and target.
        // That should be a use-in-hand event instead.
        // If this is not the case, can lead to bugs (e.g., attempting to merge a item stack into itself).
        DebugTools.Assert(used != target);

        User = user;
        Used = used;
        Target = target;
        ClickLocation = clickLocation;
    }
}

