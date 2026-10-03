namespace Content.Server._Funkystation.HandheldNewsReader;

/// <summary>
/// marks item as a standalone newsreader
/// </summary>
[RegisterComponent]
public sealed partial class HandheldNewsReaderComponent : Component
{
    [DataField]
    public int ArticleNumber;
}
