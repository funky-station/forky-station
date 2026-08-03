using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._Funkystation.Documents;

/// <summary>
/// A single standardized document that a DocumentPrinter can spawn as a
/// pre-filled paper entity.
/// </summary>
[Prototype]
public sealed partial class DocumentPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = null!;

    /// <summary>
    /// .ftl for the document's name in the UI.
    /// </summary>
    [DataField("name", required: true)]
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// .ftl for the short description shown under the name in the UI
    /// </summary>
    [DataField("description")]
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Which category tab this document is filed under. Documents with no explicit category fall back to the Misc category
    /// </summary>
    [DataField("category", customTypeSerializer: typeof(PrototypeIdSerializer<DocumentCategoryPrototype>))]
    public string Category { get; private set; } = "DocCatMisc";

    /// <summary>
    /// The paper entity prototype to spawn on print
    /// </summary>
    [DataField("paperPrototype", required: true)]
    public EntProtoId PaperPrototype { get; private set; }

    /// <summary>
    /// .ftl for the body text written onto the spawned paper
    /// </summary>
    [DataField("content", required: true)]
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Stamp prototype automatically applied to the paper on print
    /// </summary>
    [DataField("stamps")]
    public List<string> Stamps { get; private set; } = new();

    /// <summary>
    /// Access tags required to print this specific document
    /// </summary>
    [DataField("access")]
    public List<string>? RequiredAccess { get; private set; }
}
