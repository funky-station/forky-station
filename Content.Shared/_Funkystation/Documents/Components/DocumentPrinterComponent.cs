using Content.Shared.Containers.ItemSlots;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared._Funkystation.Documents.Components;

/// <summary>
/// Marks an entity as a document printer. The set of documents available is per prototype.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DocumentPrinterComponent : Component
{
    /// <summary>
    /// Document prototypes this specific printer entity can print
    /// </summary>
    [DataField("availableDocuments", customTypeSerializer: typeof(PrototypeIdListSerializer<DocumentPrototype>))]
    public List<string> AvailableDocuments = new();

    /// <summary>
    /// Additional documents unlocked once this printer has been emagged
    /// </summary>
    [DataField("emagDocuments", customTypeSerializer: typeof(PrototypeIdListSerializer<DocumentPrototype>))]
    public List<string> EmagDocuments = new();

    /// <summary>
    /// Additional documents unlocked once the manager wire has been cut
    /// </summary>
    [DataField("managerDocuments", customTypeSerializer: typeof(PrototypeIdListSerializer<DocumentPrototype>))]
    public List<string> ManagerDocuments = new();

    /// <summary>
    /// Delay between pressing print and the paper appearing
    /// </summary>
    [DataField("printDelay")]
    public TimeSpan PrintDelay = TimeSpan.FromSeconds(4);

    /// <summary>
    /// Minimum time between separate print jobs on this printer
    /// </summary>
    [DataField("printCooldown")]
    public TimeSpan PrintCooldown = TimeSpan.FromSeconds(6);

    /// <summary>
    /// The game time before which further print requests on this printer are rejected
    /// </summary>
    [ViewVariables]
    public TimeSpan NextPrintTime;

    /// <summary>
    /// True once the manager wire has been cut
    /// </summary>
    [ViewVariables]
    public bool ManagerWireCut;

    /// <summary>
    /// True once access bypassed
    /// </summary>
    [ViewVariables]
    public bool AccessBroken;

    [DataField("printSound")]
    public SoundSpecifier PrintSound = new SoundPathSpecifier("/Audio/Machines/printer.ogg");

    /// <summary>
    /// Slot paper is inserted into to be copied
    /// </summary>
    [ViewVariables]
    public ItemSlot PaperSlot = new();

    /// <summary>
    /// Fallback paper prototype just in case...
    /// </summary>
    [DataField]
    public EntProtoId CopyPaperId = "Paper";

    /// <summary>
    /// 1 in number chance of a paper jam on each completed print or copy job. 0 or less disables jams.
    /// </summary>
    [DataField]
    public int JamOneInChance = 60;

    /// <summary>
    /// True while jammed. Blocks all printing until cleared
    /// </summary>
    [ViewVariables]
    public bool Jammed;

    /// <summary>
    /// How long the doAfter to clear a jam takes
    /// </summary>
    [DataField]
    public TimeSpan JamClearDelay = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Tool quality required to clear a jam
    /// </summary>
    [DataField]
    public ProtoId<ToolQualityPrototype> JamClearToolQuality = "Anchoring";

    /// <summary>
    /// Sound when a jam happens
    /// </summary>
    [DataField]
    public SoundSpecifier JamSound = new SoundPathSpecifier("/Audio/_Funkystation/Machines/document_printer_jam.ogg");

    /// <summary>
    /// Sound during the doAfter clearing the jam
    /// </summary>
    [DataField]
    public SoundSpecifier JamLoopSound = new SoundPathSpecifier("/Audio/_Funkystation/Machines/document_printer_unjam_loop.ogg");

    /// <summary>
    /// Handle to the currently-playing unjam loop sound, so it can be stopped on doAfter end
    /// </summary>
    [ViewVariables]
    public EntityUid? JamLoopSoundEntity;
}
