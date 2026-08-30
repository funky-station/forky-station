namespace Content.Shared._Funkystation.Traits.Unions;

[RegisterComponent]
public sealed partial class UnionClipboardComponent : Component
{
    [DataField] public string GroupingId = string.Empty;
    public EntityUid? PendingStewardCandidate;
}
