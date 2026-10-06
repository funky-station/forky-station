namespace Content.Shared._Funkystation.Molotov.Components;

// marker for solution containers that can be turned into a molotov with a rag
[RegisterComponent]
public sealed partial class MolotovMaterialComponent : Component
{
    // solution to move into the crafted molotov
    [DataField]
    public string Solution = "drink";

    [DataField]
    public float CraftDelay = 4f;
}
