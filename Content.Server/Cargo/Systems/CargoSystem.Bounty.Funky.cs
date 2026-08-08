using System.Linq;
using Content.Server.Cargo.Components;
using Content.Shared._Funkystation.Cargo.Prototypes;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Cargo;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Research.Components;
using Content.Shared.Research.Systems;
using JetBrains.Annotations;
using Robust.Shared.Random;

namespace Content.Server.Cargo.Systems;

public sealed partial class CargoSystem
{
    [Dependency] private SharedResearchSystem _research = default!;
    [Dependency] private SharedSolutionContainerSystem _solutionContainer = default!;

    /// <summary>
    /// Determines whether the <paramref name="entity"/> meets the criteria for the bounty <paramref name="reagentBounty"/>.
    /// </summary>
    /// <param name="entity">Some given entity to be checked against criteria</param>
    /// <param name="reagentBounty">The specific bounty reagent item that is being checked against</param>
    /// <returns>true if <paramref name="entity"/> is a valid item for the bounty entry, otherwise false</returns>
    public bool IsValidBountyEntry(EntityUid entity, CargoReagentBountyItemData reagentBounty)
    {
        if (!TryComp<SolutionComponent>(entity, out var solutions))
            return false;

        if (!_protoMan.TryIndex(reagentBounty.Reagent, out var bounty))
            return false;

        foreach (var sol in solutions.Solution.Contents)
        {
            if (sol.Reagent.Prototype.Equals(bounty.ID))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the <paramref name="entity"/> meets the criteria for the bounty <paramref name="gasBounty"/>.
    /// </summary>
    /// <param name="entity">Some given entity to be checked against criteria</param>
    /// <param name="gasBounty">The specific bounty gas that is being checked against</param>
    /// <returns>true if <paramref name="entity"/> is a valid item for the bounty entry, otherwise false</returns>
    public bool IsValidBountyEntry(EntityUid entity, CargoGasBountyItemData gasBounty)
    {
        // Currently checking components separately since I don't know a method to query for interfaces.
        // Please replace if a better method is made / found
        if (TryComp<GasTankComponent>(entity, out var gasTank))
        {
            var gases = gasTank.Air;

            return gases.GetMoles(gasBounty.Gas) > 0;
        }

        if (TryComp<GasCanisterComponent>(entity, out var gasCan))
        {
            var gases = gasCan.Air;

            return gases.GetMoles(gasBounty.Gas) > 0;
        }
        return false;
    }

    /// <summary>
    /// Determines whether the <paramref name="entity"/> meets the criteria for the bounty <paramref name="entry"/>.
    /// </summary>
    /// <returns>true if <paramref name="entity"/> is a valid item for the bounty entry, otherwise false</returns>
    public bool IsValidBountyEntry(EntityUid entity, ICargoBountyEntry entry)
    {
        return entry switch
        {
            CargoBountyItemEntry objectBounty =>
                IsValidBountyEntry(entity, new CargoObjectBountyItemData(objectBounty)),
            CargoBountyReagentEntry reagentBounty =>
                IsValidBountyEntry(entity, new CargoReagentBountyItemData(reagentBounty)),
            CargoBountyGasEntry gasBounty =>
                IsValidBountyEntry(entity, new CargoGasBountyItemData(gasBounty)),
            _ => throw new NotImplementedException($"Unknown type: {entry.GetType().Name}"),
        };
    }

    /// <summary>
    /// This method will attempt to add a bounty to a given station bounty database
    /// </summary>
    /// <param name="uid">The uid of the entity trying to add the item, this is normally the bounty computer</param>
    /// <param name="component">The bounty database for a station, each station has one though we normally don't have
    /// any outside the main station</param>
    /// <returns>True if the bounty is successfully added, false otherwise</returns>
    /// <exception cref="NotImplementedException">This will be thrown if some bounty type that handling has not be
    /// created for is attempted to be made</exception>
    [PublicAPI]
    public bool TryAddRandomBounty(EntityUid uid, StationCargoBountyDatabaseComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return false;

        if (component.Bounties.Count >= component.MaxBounties)
            return false;

        var allBounties = _protoMan.EnumeratePrototypes<CargoBountyCategoryPrototype>().ToList();
        if (allBounties.Count < 1)
            return false;
        var chosenCategory = false;
        var bountyCategory = _random.Pick(allBounties);
        List<ICargoBountyEntry> bountyItems = [];
        while (!chosenCategory)
        {
            // This all feels wrong, but it works so hey ho
            var duplicheck = true;
            while (duplicheck)
            {
                duplicheck = false;
                bountyCategory = _random.Pick(allBounties);
                foreach (var entry in component.Bounties)
                {
                    if (entry.Category == bountyCategory.Name)
                    {
                        duplicheck = true;
                    }
                }
            }

            chosenCategory = CheckCategory(uid, bountyCategory, out var availableBounties);
            if (chosenCategory)
            {
                bountyItems = availableBounties;
                continue;
            }

            allBounties.Remove(bountyCategory);

            if (allBounties.Count == 0)
            {
                Log.Error("Failed to add bounty because there are no categories available");
                return false;
            }

            bountyCategory = _random.Pick(allBounties);
        }

        var totalItems = bountyCategory.MaxTargets == 0 ? bountyItems.Count : bountyCategory.MaxTargets;

        // Smaller number means that there will be on average less item per bounty
        const double itemNumberWeight = 0.9;
        var selection = Math.Min(1 - Math.Ceiling(Math.Log(Math.Pow(_random.NextDouble(), itemNumberWeight), 2)),
            totalItems);
        var totalReward = 0;
        var newBountyObjectives = new List<CargoBountyItemData>();
        var totalBountyItems = 0;

        for (var i = 1; i <= selection;)
        {
            if (!SelectBountyEntry(bountyItems, out var bountyItem))
            {
                return false;
            }

            var skip = false;
            foreach (var entry in newBountyObjectives)
            {
                if (entry.Name == bountyItem.Name)
                {
                    skip = true;
                }
            }
            if (skip)
                continue;

            CargoBountyItemData bountyItemData = bountyItem switch
            {
                CargoBountyItemEntry itemEntry => new CargoObjectBountyItemData(itemEntry),
                CargoBountyReagentEntry itemEntry => new CargoReagentBountyItemData(itemEntry),
                CargoBountyGasEntry itemEntry => new CargoGasBountyItemData(itemEntry),
                _ => throw new NotImplementedException($"Unknown type: {bountyItem.GetType().Name}"),
            };

            var steps = (bountyItem.MaxAmount - bountyItem.MinAmount) / bountyItem.AmountStep;
            var step = _random.Next(steps + 1);
            var bountyAmount = step * bountyItem.AmountStep + bountyItem.MinAmount;
            totalReward += bountyAmount * bountyItem.RewardPer;
            bountyItemData.Amount = bountyAmount;

            // Counter for the total number of bounty items, used for if the number goes over 30 (basic crate limit)
            switch (bountyItemData)
            {
                case CargoObjectBountyItemData objectBounty:
                    totalBountyItems += bountyAmount;
                    break;
                case CargoReagentBountyItemData reagentBounty:
                    totalBountyItems ++;
                    break;
                case CargoGasBountyItemData gasBounty:
                    totalBountyItems ++;
                    break;
            }

            newBountyObjectives.Add(bountyItemData);
            if (totalItems > 1)
                totalItems--;

            i++;
        }


        var newBountyIdPrefix = bountyCategory.IdPrefix;
        var newBountyCategory = bountyCategory.Name;
        // newBounty.Reward = totalReward;
        _nameIdentifier.GenerateUniqueName(uid, BountyNameIdentifierGroup, out var newBountyId);
        var newBountyDescription = Loc.GetString("bounty-console-category-description",
            ("category", Loc.GetString(bountyCategory.Name)),
            ("id", newBountyId));

        if (totalBountyItems > 30)
        {
            newBountyDescription += " " + Loc.GetString("bounty-console-storage-warning");
        }
        if (component.Bounties.Any(b => b.Id == $"{newBountyIdPrefix}{newBountyId:D3}"))
        {
            Log.Error("Failed to add bounty {ID} because another one with the same ID already existed!", newBountyId);
            return false;
        }

        var newBounty = new CargoBountyData(newBountyId, totalReward, newBountyDescription, newBountyObjectives, newBountyCategory, newBountyIdPrefix);

        component.Bounties.Add(newBounty);
        component.TotalBounties++;
        return true;
    }

    /// <summary>
    /// Selects a bounty item from a list of entries accounting for the entries weightings.
    /// </summary>
    /// <param name="entries">List of entries to select from.</param>
    /// <param name="bountyEntry">The randomly selected entry.</param>
    /// <returns>True of false depending on the success of the selection.</returns>
    private bool SelectBountyEntry(List<ICargoBountyEntry> entries, out ICargoBountyEntry bountyEntry)
    {
        double totalWeight = 0;
        foreach (var entry in entries)
        {
            totalWeight += entry.Weight;
        }
        var roll = _random.NextDouble(0, totalWeight);

        foreach (var entry in entries)
        {
            roll -= entry.Weight;
            if (!(roll <= 0))
                continue;
            bountyEntry = entry;
            return true;
        }

        bountyEntry = new CargoBountyItemEntry();
        return false;
    }

    /// <summary>
    /// Checks if a given bounty category is valid to be created for and returns a list of valid objectives from the
    /// category that can have bounties created from
    /// </summary>
    /// <param name="uid">The entity try to create a bounty</param>
    /// <param name="category">Some given bounty category as defined in yml</param>
    /// <param name="availableBounties">Returns a list of currently valid objectives</param>
    /// <returns>True if the category can have bounties created for, false otherwise</returns>
    private bool CheckCategory(EntityUid uid, CargoBountyCategoryPrototype category, out List<ICargoBountyEntry> availableBounties)
    {
        var bountyItems = new List<ICargoBountyEntry>(category.Entries);
        List<ICargoBountyEntry> toRemove = new();
        foreach (var bountyEntry in bountyItems)
        {
            switch (bountyEntry)
            {
                case CargoBountyItemEntry bountyItem:
                    if (bountyItem.RequiredResearch == null)
                        continue;

                    List<bool> techChecks = [];
                    foreach (var research in bountyItem.RequiredResearch)
                    {

                        var query = EntityManager.EntityQueryEnumerator<TechnologyDatabaseComponent>();

                        while (query.MoveNext(out var tEntityUid, out var technologyDatabaseComponent))
                        {
                            if (_station.GetOwningStation(uid) is { } station &&
                                _station.GetOwningStation(tEntityUid) != station)
                                continue;
                            techChecks.Add(
                                _research.IsTechnologyUnlocked(tEntityUid, (string) research, technologyDatabaseComponent));
                            break;
                        }
                    }

                    if (techChecks.Count == 0 || !techChecks.Any(techCheck => techCheck))
                    {
                        toRemove.Add(bountyItem);
                    }
                    break;
                case CargoBountyReagentEntry bountyItem:
                    continue;
            }
        }

        bountyItems.RemoveAll(b => toRemove.Contains(b));

        if (bountyItems.Count == 0)
        {
            availableBounties = [];
            return false;
        }

        availableBounties = bountyItems;
        return true;
    }
}
