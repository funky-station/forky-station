using Robust.Shared.GameStates;

namespace Content.Shared._AS.EditableMetadata;

/// <summary>
/// Stores optional overrides for an entity's name and description.
/// Other systems may provide an interface for editing these values.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ASEditableMetadataComponent : Component
{
    public const int AbsoluteMaxNameLength = 64;
    public const int AbsoluteMaxDescriptionLength = 256;

    /// <summary>
    /// Maximum custom name length accepted by an editing system.
    /// </summary>
    [DataField]
    public int MaxNameLength = AbsoluteMaxNameLength;

    /// <summary>
    /// Maximum custom description length accepted by an editing system.
    /// </summary>
    [DataField]
    public int MaxDescriptionLength = AbsoluteMaxDescriptionLength;

    /// <summary>
    /// An empty value preserves the entity prototype's name.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string CustomName = string.Empty;

    /// <summary>
    /// An empty value preserves the entity prototype's description.
    /// </summary>
    [DataField, AutoNetworkedField]
    public string Description = string.Empty;
}
