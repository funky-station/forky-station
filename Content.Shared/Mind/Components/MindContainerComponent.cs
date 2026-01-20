// SPDX-FileCopyrightText: 2026 ScarKy0
// SPDX-FileCopyrightText: 2024 Errant
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2021 ShadowCommander
// SPDX-FileCopyrightText: 2023 ShadowCommander
// SPDX-FileCopyrightText: 2018 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2020 Paul Ritter
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2021 Fishfish458
// SPDX-FileCopyrightText: 2021 E F R
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2021 Javier Guardia Fernández
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 NuclearWinter
// SPDX-FileCopyrightText: 2020 SoulSloth
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto
// SPDX-FileCopyrightText: 2020 Exp
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2020 Remie Richards
// SPDX-FileCopyrightText: 2020 Hugal31
// SPDX-FileCopyrightText: 2020 zumorica
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-FileCopyrightText: 2018 PJB3005
// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameStates;

namespace Content.Shared.Mind.Components;

/// <summary>
/// This component indicates that this entity may have mind, which is simply an entity with a <see cref="MindComponent"/>.
/// The mind entity is not actually stored in a "container", but is simply stored in nullspace.
/// </summary>
[RegisterComponent, Access(typeof(SharedMindSystem)), NetworkedComponent, AutoGenerateComponentState]
public sealed partial class MindContainerComponent : Component
{
    /// <summary>
    ///     The mind controlling this mob. Can be null.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Mind;

    /// <summary>
    ///     True if we have a mind, false otherwise.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Mind))]
    public bool HasMind => Mind != null;

    /// <summary>
    ///     Whether the mind will be put on a ghost after this component is shutdown.
    /// </summary>
    [DataField]
    public bool GhostOnShutdown = true;
}

public abstract class MindEvent : EntityEventArgs
{
    public readonly Entity<MindComponent> Mind;
    public readonly Entity<MindContainerComponent> Container;

    public MindEvent(Entity<MindComponent> mind, Entity<MindContainerComponent> container)
    {
        Mind = mind;
        Container = container;
    }
}

/// <summary>
/// Event raised directed at a mind-container when a mind gets removed.
/// </summary>
public sealed class MindRemovedMessage : MindEvent
{
    public MindRemovedMessage(Entity<MindComponent> mind, Entity<MindContainerComponent> container)
        : base(mind, container)
    {
    }
}

/// <summary>
/// Event raised directed at a mind when it gets removed from a mind-container.
/// </summary>
public sealed class MindGotRemovedEvent : MindEvent
{
    public MindGotRemovedEvent(Entity<MindComponent> mind, Entity<MindContainerComponent> container)
        : base(mind, container)
    {
    }
}

/// <summary>
/// Event raised directed at a mind-container when a mind gets added.
/// </summary>
public sealed class MindAddedMessage : MindEvent
{
    public MindAddedMessage(Entity<MindComponent> mind, Entity<MindContainerComponent> container)
        : base(mind, container)
    {
    }
}

/// <summary>
/// Event raised directed at a mind when it gets added to a mind-container.
/// </summary>
public sealed class MindGotAddedEvent : MindEvent
{
    public MindGotAddedEvent(Entity<MindComponent> mind, Entity<MindContainerComponent> container)
        : base(mind, container)
    {
    }
}
