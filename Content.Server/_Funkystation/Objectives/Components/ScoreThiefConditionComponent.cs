using Content.Server._Funkystation.Objectives.Systems;

namespace Content.Server._Funkystation.Objectives.Components;

/// <summary>
/// Requires that you steal enough to fill a speso quota
/// </summary>
[RegisterComponent, Access(typeof(ScoreThiefConditionSystem))]
public sealed partial class ScoreThiefConditionComponent : Component
{
    [DataField(required: true)]
    public int AmountSpesos;

    [DataField(required: true)]
    public int AmountSpesosVariance;

    [DataField]
    public int AmountSpesosVarianceInterval = 1000;

    [DataField]
    public bool CheckStealAreas = true;

    public int TargetScore = 0;
    public int CurrentScore = 0;
}
