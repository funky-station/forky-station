namespace Content.Shared._Funkystation.Traits.Unions;

[RegisterComponent]
public sealed partial class UnionCardComponent : Component
{
    public EntityUid? OwnerUid;
    public string OwnerName = string.Empty;
    public string UnionName = string.Empty;
    public string Position = string.Empty;
}
