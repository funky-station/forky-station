using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.CateringDigiboard.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class CateringDigiboardComponent : Component
{
    /// <summary>
    /// the menu currently being designed, items and category markers in print order
    /// </summary>
    [ViewVariables]
    public readonly List<CateringMenuItem> Items = [];

    /// <summary>
    /// counter used to hand out unique row IDs as entries are added
    /// </summary>
    [ViewVariables]
    public int NextItemId = 1;

    /// <summary>
    /// how many lines a menu can have. category markers don't count
    /// </summary>
    [DataField]
    public int MaxItems = 20;

    /// <summary>
    /// prototype paper spawned
    /// </summary>
    [DataField]
    public EntProtoId MenuPaperId = "PaperCateringMenu";

    /// <summary>
    /// how many characters wide a line is, used to center category headers with leading spaces later (I really wish we had a center function)
    /// </summary>
    [DataField]
    public int MenuLineWidth = 55;

    /// <summary>
    /// copies still left from the current print job
    /// </summary>
    [ViewVariables]
    public int PendingCopies;

    /// <summary>
    /// how many papers overflowed the storage and fell onto the floor
    /// </summary>
    [ViewVariables]
    public int PendingDropped;

    /// <summary>
    /// current menu for the batch printing
    /// </summary>
    [ViewVariables]
    public string PendingContent = string.Empty;

    /// <summary>
    /// who printed current batch
    /// </summary>
    [ViewVariables]
    public EntityUid PendingActor;

    /// <summary>
    /// time at which the next queued copy should print
    /// </summary>
    [ViewVariables]
    public TimeSpan NextPrintTime;

    /// <summary>
    /// how long each copy takes to print
    /// </summary>
    [DataField]
    public TimeSpan PrintInterval = TimeSpan.FromSeconds(3);

    /// <summary>
    /// sound played when printing
    /// </summary>
    [DataField]
    public SoundSpecifier PrintSound = new SoundPathSpecifier("/Audio/Machines/printer.ogg");
}
