// SPDX-FileCopyrightText: 2025 IProduceWidgets
// SPDX-FileCopyrightText: 2020 metalgearsloth
// SPDX-FileCopyrightText: 2021 metalgearsloth
// SPDX-FileCopyrightText: 2022 metalgearsloth
// SPDX-FileCopyrightText: 2023 metalgearsloth
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2024 Tayrtahn
// SPDX-FileCopyrightText: 2024 Just-a-Unity-Dev
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2024 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2023 Nemanja
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2024 AJCM-git
// SPDX-FileCopyrightText: 2023 deltanedas
// SPDX-FileCopyrightText: 2021 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2023 Leon Friedrich
// SPDX-FileCopyrightText: 2023 TemporalOroboros
// SPDX-FileCopyrightText: 2021 Ygg01
// SPDX-FileCopyrightText: 2023 Ygg01
// SPDX-FileCopyrightText: 2022 keronshb
// SPDX-FileCopyrightText: 2023 keronshb
// SPDX-FileCopyrightText: 2023 Visne
// SPDX-FileCopyrightText: 2023 Joel Hedtjärn
// SPDX-FileCopyrightText: 2022 Kara
// SPDX-FileCopyrightText: 2023 Kara
// SPDX-FileCopyrightText: 2022 Rapidgame7
// SPDX-FileCopyrightText: 2022 ike709
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 Júlio César Ueti
// SPDX-FileCopyrightText: 2022 Alex Evgrashin
// SPDX-FileCopyrightText: 2022 Fishfish458
// SPDX-FileCopyrightText: 2021 mirrorcult
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 20kdc
// SPDX-FileCopyrightText: 2018 Acruid
// SPDX-FileCopyrightText: 2020 Acruid
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 chairbender
// SPDX-FileCopyrightText: 2019 Silver
// SPDX-License-Identifier: MIT

using Content.Shared.Explosion;
using Content.Shared.Hands;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
namespace Content.Server.Storage.EntitySystems;

public sealed partial class StorageSystem : SharedStorageSystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StorageComponent, BeforeExplodeEvent>(OnExploded);
        SubscribeLocalEvent<StorageFillComponent, MapInitEvent>(OnStorageFillMapInit);
    }

    private void OnExploded(Entity<StorageComponent> ent, ref BeforeExplodeEvent args)
    {
        args.Contents.AddRange(ent.Comp.Container.ContainedEntities);
    }

    /// <inheritdoc />
    public override void PlayPickupAnimation(EntityUid uid, EntityCoordinates initialCoordinates, EntityCoordinates finalCoordinates,
        Angle initialRotation, EntityUid? user = null)
    {
        var filter = Filter.Pvs(uid).RemoveWhereAttachedEntity(e => e == user);
        RaiseNetworkEvent(new PickupAnimationEvent(GetNetEntity(uid), GetNetCoordinates(initialCoordinates), GetNetCoordinates(finalCoordinates), initialRotation), filter);
    }
}
