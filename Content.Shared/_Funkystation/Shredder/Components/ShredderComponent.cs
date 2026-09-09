using Content.Shared.Containers.ItemSlots;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Shredder.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class ShredderComponent : Component
{
    /// <summary>
    /// whether the shredder is currently switched on, toggled via verb
    /// </summary>
    [DataField, ViewVariables]
    public bool Enabled = true;

    /// <summary>
    /// whether the shredder is actively shredding an item
    /// </summary>
    [ViewVariables]
    public bool Shredding;

    /// <summary>
    /// slot the bin sits in
    /// </summary>
    [ViewVariables]
    public ItemSlot BinSlot = new();

    /// <summary>
    /// what counts as shreddable
    /// </summary>
    [DataField]
    public EntityWhitelist? Whitelist;

    /// <summary>
    /// how long the shredding animation plays before the machine returns to idle
    /// </summary>
    [DataField]
    public TimeSpan ShredDelay = TimeSpan.FromSeconds(7.5);

    /// <summary>
    /// sound played while shredding
    /// </summary>
    [DataField]
    public SoundSpecifier ShredSound = new SoundPathSpecifier("/Audio/_Funkystation/Machines/paper_shredder.ogg");

    /// <summary>
    /// item spawned when shredding is complete
    /// </summary>
    [DataField]
    public EntProtoId FloorPilePrototype = "ShreddedPaperPile";
}
