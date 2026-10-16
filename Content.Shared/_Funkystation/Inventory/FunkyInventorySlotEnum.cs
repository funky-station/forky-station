namespace Content.Shared._Funkystation.Inventory;
/// <summary>
/// <para> The type of an inventory (hotbar) slot. </para>
/// <para> Specifies behavior like location on the HUD and whitelist as well as human-readable naming for the storage.</para>
/// </summary>
[Flags, Serializable]
public enum FunkyInventorySlotEnum
{
    Head =          1 << 00,
    Eyes =          1 << 01,
    Ears =          1 << 02,
    Mask =          1 << 03,
    OuterClothing = 1 << 04,
    InnerClothing = 1 << 05,
    Neck =          1 << 06,
    Back =          1 << 07,
    Belt =          1 << 08,
    Gloves =        1 << 09,
    Pocket =        1 << 10,
    Feet =          1 << 11,
    SuitStorage =   1 << 12,
    Weapon =        1 << 13, // Funky slot type. If you want to disable this, find where it's used in YAML and remove it there. Do NOT comment it out here.
}
