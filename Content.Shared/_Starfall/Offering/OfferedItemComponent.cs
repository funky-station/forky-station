using Robust.Shared.GameStates;

namespace Content.Shared._Starfall.Offering;

/// <summary>
/// Marks a held item as being offered by its holder to a specific recipient.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class OfferedItemComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid Offerer;

    [DataField, AutoNetworkedField]
    public EntityUid Recipient;
}
