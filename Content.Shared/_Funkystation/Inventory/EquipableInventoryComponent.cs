using Content.Shared.Whitelist;
using Robust.Shared.Containers;

namespace Content.Shared._Funkystation.Inventory;

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Inventory;

/// <summary>
/// Used to give an equipable item an inventory to be added to the player's hotbar
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(InventorySystem))]
public sealed partial class EquipableInventoryComponent : Component
{
    /// <summary>
    /// The template defining how the inventory layout
    /// </summary>
    [DataField, AutoNetworkedField]
    public ProtoId<InventoryTemplatePrototype> TemplateId = "jumpsuitBase";
}
