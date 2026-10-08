using Content.Shared.Radio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Radio;

// Realized far too late that this is basically a specialized ColorPalettePrototype
[Prototype]
public sealed partial class RadioChannelColorsPrototype : IPrototype, IComparable<RadioChannelColorsPrototype>
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = null!;

    /// <summary>
    /// The localized name of the color preset, to be displayed in the settings menu.
    /// </summary>
    [DataField(required: true)]
    public LocId Name { get; private set; } = string.Empty;

    /// <summary>
    /// The radio channels and their associated colors specified by this preset.
    /// </summary>
    [DataField(required: true)]
    public required Dictionary<ProtoId<RadioChannelPrototype>, Color> Colors;

    /// <summary>
    /// The point in the list in the player settings the preset should be sorted at.
    /// </summary>
    [DataField]
    public int Order = 0;

    public int CompareTo(RadioChannelColorsPrototype? other)
    {
        return Order.CompareTo(other?.Order);
    }
}
