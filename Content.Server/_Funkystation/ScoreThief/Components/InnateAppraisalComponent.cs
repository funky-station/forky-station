using Content.Server._Funkystation.ScoreThief.Systems;

namespace Content.Server._Funkystation.ScoreThief.Components;

/// <summary>
/// Gives an entity the innate ability to appraise items it looks at
/// </summary>
[RegisterComponent, Access(typeof(InnateAppraisalSystem))]
public sealed partial class InnateAppraisalComponent : Component
{
    // Use ScoreThiefPriceModifierComponent?
    [DataField]
    public bool BlackMarket = false;
}
