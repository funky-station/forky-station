// SPDX-FileCopyrightText: 2019 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers
// SPDX-FileCopyrightText: 2025 Whatstone
// SPDX-FileCopyrightText: 2025 Tayrtahn
// SPDX-FileCopyrightText: 2020 DrSmugleaf
// SPDX-FileCopyrightText: 2021 DrSmugleaf
// SPDX-FileCopyrightText: 2023 DrSmugleaf
// SPDX-FileCopyrightText: 2022 Nemanja
// SPDX-FileCopyrightText: 2022 Leon Friedrich
// SPDX-FileCopyrightText: 2022 Rane
// SPDX-FileCopyrightText: 2022 mirrorcult
// SPDX-FileCopyrightText: 2022 wrexbe
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto
// SPDX-FileCopyrightText: 2021 Visne
// SPDX-FileCopyrightText: 2021 Acruid
// SPDX-FileCopyrightText: 2020 FL-OZ
// SPDX-FileCopyrightText: 2019 ZelteHonor
// SPDX-FileCopyrightText: 2019 Víctor Aguilera Puerto
// SPDX-License-Identifier: MIT

using Content.Shared.Research.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe;

[Serializable, NetSerializable]
public sealed class LatheUpdateState : BoundUserInterfaceState
{
    public List<ProtoId<LatheRecipePrototype>> Recipes;

    public LatheRecipeBatch[] Queue;

    public ProtoId<LatheRecipePrototype>? CurrentlyProducing;

    public LatheUpdateState(List<ProtoId<LatheRecipePrototype>> recipes, LatheRecipeBatch[] queue, ProtoId<LatheRecipePrototype>? currentlyProducing = null)
    {
        Recipes = recipes;
        Queue = queue;
        CurrentlyProducing = currentlyProducing;
    }
}

/// <summary>
///     Sent to the server to sync material storage and the recipe queue.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheSyncRequestMessage : BoundUserInterfaceMessage
{

}

/// <summary>
///     Sent to the server when a client queues a new recipe.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheQueueRecipeMessage : BoundUserInterfaceMessage
{
    public readonly string ID;
    public readonly int Quantity;
    public LatheQueueRecipeMessage(string id, int quantity)
    {
        ID = id;
        Quantity = quantity;
    }
}

/// <summary>
///     Sent to the server to remove a batch from the queue.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheDeleteRequestMessage(int index) : BoundUserInterfaceMessage
{
    public int Index = index;
}

/// <summary>
///     Sent to the server to move the position of a batch in the queue.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheMoveRequestMessage(int index, int change) : BoundUserInterfaceMessage
{
    public int Index = index;
    public int Change = change;
}

/// <summary>
///     Sent to the server to stop producing the current item.
/// </summary>
[Serializable, NetSerializable]
public sealed class LatheAbortFabricationMessage() : BoundUserInterfaceMessage
{
}

[NetSerializable, Serializable]
public enum LatheUiKey
{
    Key,
}
