using Content.Shared.Atmos;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Prototypes;

public interface ICargoBountyEntry
{
    /// <summary>
    /// How much of the item must be present to satisfy the entry
    /// </summary>
    [DataField]
    public int Amount { get; init; }

    // Beginning of Funky Station edits
    /// <summary>
    /// A minimum amount of the item that can be requested in a bounty, used to make sure a bounty isn't to underwhelming
    /// </summary>
    [DataField]
    public int MinAmount { get; init; }

    /// <summary>
    /// A maximum amount of the item that can be requested for a bounty
    /// </summary>
    [DataField]
    public int MaxAmount { get; init; }

    /// <summary>
    /// The step size for the bounties amount, i.e. min:1 max:3 step:2 means only amounts 1 and 3 will be generated.
    /// </summary>
    [DataField]
    public int AmountStep { get; init; }

    /// <summary>
    /// The amount each item will reward for a bounty
    /// </summary>
    [DataField]
    public int RewardPer { get; init; }

    /// <summary>
    /// A player-facing name for the item. Assigned here but declared in the cargo bounties.ftl file.
    /// </summary>
    [DataField]
    public LocId Name { get; init; }

    /// <summary>
    /// Some weight that can be used to effect the chances an item is selected, default is 1, smaller number means less
    /// likely, higher more likely.
    /// </summary>
    [DataField]
    public double Weight { get; init; }
}

[DataDefinition, Serializable, NetSerializable]
public readonly partial record struct CargoBountyReagentEntry : ICargoBountyEntry
{
    /// <summary>
    /// What reagent will satisfy the entry.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<ReagentPrototype> Reagent { get; init; }

    [DataField]
    public int Amount { get; init; } = 1;
    [DataField]
    public int MinAmount { get; init; } = 1;
    [DataField]
    public int MaxAmount { get; init; } = 1;
    [DataField]
    public int AmountStep { get; init; } = 1;
    [DataField]
    public int RewardPer { get; init; } = 1;
    [DataField]
    public LocId Name { get; init; } = "";
    [DataField]
    public double Weight { get; init; } = 1;
}

[DataDefinition, Serializable, NetSerializable]
public readonly partial record struct CargoBountyGasEntry : ICargoBountyEntry
{
    /// <summary>
    /// What gas reagent will satisfy the entry.
    /// I hate gases, this needs to be set as per the entries in the Gas enum in <see cref="Content.Shared.Atmos.Atmospherics"/>
    /// I pray someone smarter than I knows a better way to do this
    /// </summary>
    [DataField(required: true)]
    public Gas Gas { get; init; }

    [DataField]
    public int Amount { get; init; } = 1;
    [DataField]
    public int MinAmount { get; init; } = 1;
    [DataField]
    public int MaxAmount { get; init; } = 1;
    [DataField]
    public int AmountStep { get; init; } = 1;
    [DataField]
    public int RewardPer { get; init; } = 1;
    [DataField]
    public LocId Name { get; init; } = "";
    [DataField]
    public double Weight { get; init; } = 1;
}
