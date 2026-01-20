// SPDX-FileCopyrightText: 2020 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2022 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 c4llv07e
// SPDX-FileCopyrightText: 2023 Chief-Engineer
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Alex Evgrashin
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Paul Ritter
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2019 DamianX
// SPDX-License-Identifier: MIT

using Content.Shared.Access.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Access.Components;

/// <summary>
///     Simple mutable access provider found on ID cards and such.
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedAccessSystem))]
[AutoGenerateComponentState]
public sealed partial class AccessComponent : Component
{
    /// <summary>
    /// True if the access provider is enabled and can grant access.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool Enabled = true;

    [DataField]
    [Access(typeof(SharedAccessSystem), Other = AccessPermissions.ReadExecute)] // FIXME Friends
    [AutoNetworkedField]
    public HashSet<ProtoId<AccessLevelPrototype>> Tags = new();

    /// <summary>
    /// Access Groups. These are added to the tags during map init. After map init this will have no effect.
    /// </summary>
    [DataField(readOnly: true)]
    [AutoNetworkedField]
    public HashSet<ProtoId<AccessGroupPrototype>> Groups = new();
}

/// <summary>
/// Event raised on an entity to find additional entities which provide access.
/// </summary>
[ByRefEvent]
public struct GetAdditionalAccessEvent
{
    public HashSet<EntityUid> Entities = new();

    public GetAdditionalAccessEvent()
    {
    }
}

[ByRefEvent]
public record struct GetAccessTagsEvent(HashSet<ProtoId<AccessLevelPrototype>> Tags, IPrototypeManager PrototypeManager)
{
    public void AddGroup(ProtoId<AccessGroupPrototype> group)
    {
        if (!PrototypeManager.Resolve<AccessGroupPrototype>(group, out var groupPrototype))
            return;

        Tags.UnionWith(groupPrototype.Tags);
    }
}
