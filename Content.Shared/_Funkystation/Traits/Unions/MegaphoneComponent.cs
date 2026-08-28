namespace Content.Shared._Funkystation.Traits.Unions;

[RegisterComponent]
public sealed partial class MegaphoneComponent : Component
{
    [DataField] public string GroupingId = string.Empty;

    [DataField] public TimeSpan ClaimLeadershipDelay = TimeSpan.FromSeconds(3);
}
