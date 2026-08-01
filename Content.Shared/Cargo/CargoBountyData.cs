using Robust.Shared.Serialization;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared.Cargo;

/// <summary>
/// A data structure for storing currently available bounties.
/// </summary>
[DataDefinition, NetSerializable, Serializable]
public readonly partial record struct CargoBountyData
{
    /// <summary>
    /// A unique id used to identify the bounty
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The prototype containing information about the bounty. TODO: TEMP FOR TEST WORK
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<CargoBountyPrototype> Bounty { get; init; } = string.Empty;

    /// <summary>
    /// The monetary reward for completing the bounty
    /// </summary>
    [DataField(required: true)]
    public int Reward { get; init; } = 0;

    /// <summary>
    /// A description for flavour purposes.
    /// </summary>
    [DataField]
    public LocId Description { get; init; } = string.Empty;

    /// <summary>
    /// The entries that must be satisfied for the cargo bounty to be complete.
    /// </summary>
    [DataField(required: true)]
    public List<CargoBountyItemData> Entries { get; init; } = new();

    /// <summary>
    /// A prefix appended to the beginning of a bounty's ID.
    /// </summary>
    [DataField]
    public string IdPrefix { get; init; } = "NT";

    public LocId Category { get; init; } = "";

    public CargoBountyData(int uniqueIdentifier, int reward, LocId description, List<CargoBountyItemData> entries, LocId category, string idPrefix = "NT")
    {
        Id = $"{IdPrefix}{uniqueIdentifier:D3}";
        Reward = reward;
        Description = description;
        Entries = entries;
        IdPrefix = idPrefix;
        Category = category;
    }

    /// <summary>
    /// Used for creating bounties via the old system with pre-defined bounties
    /// </summary>
    /// <param name="uniqueIdentifier">Some number to be used as an ID with IdPrefix</param>
    /// <param name="prototype">The prototype of the bounty to be created</param>
    public CargoBountyData(CargoBountyPrototype prototype, int uniqueIdentifier)
    {
        Bounty = prototype.ID;
        Id = $"{IdPrefix}{uniqueIdentifier:D3}";
        Description = prototype.Description;
        IdPrefix = prototype.IdPrefix;
        Reward = prototype.Reward;
        var items = new List<CargoBountyItemData>();
        foreach (var entry in prototype.Entries)
        {
            CargoBountyItemData newItem = entry switch
            {
                CargoObjectBountyItemEntry itemEntry => new CargoObjectBountyItemData(itemEntry),
                CargoReagentBountyItemEntry itemEntry => new CargoReagentBountyItemData(itemEntry),
                CargoGasBountyItemEntry itemEntry => new CargoGasBountyItemData(itemEntry),
                _ => throw new NotImplementedException($"Unknown type: {entry.GetType().Name}"),
            };
            items.Add(newItem);
        }
        Entries = items;
    }

    public CargoBountyData()
    {

    }
}
