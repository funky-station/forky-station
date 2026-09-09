using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.CateringDigiboard;

[Serializable, NetSerializable]
public enum CateringDigiboardUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class CateringMenuItem(string id, string name, string description, int scripCost, bool isCategory = false)
{
    public readonly string Id = id;
    public string Name = name;
    public string Description = description;
    public int ScripCost = scripCost;
    public readonly bool IsCategory = isCategory;
}

[Serializable, NetSerializable]
public sealed class CateringDigiboardBoundUserInterfaceState(List<CateringMenuItem> items, int maxItems, bool isPrinting)
    : BoundUserInterfaceState
{
    public readonly List<CateringMenuItem> Items = items;
    public readonly int MaxItems = maxItems;
    public readonly bool IsPrinting = isPrinting;
}

[Serializable, NetSerializable]
public sealed class CateringDigiboardAddItemMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class CateringDigiboardAddCategoryMessage : BoundUserInterfaceMessage;

[Serializable, NetSerializable]
public sealed class CateringDigiboardRemoveItemMessage(string itemId) : BoundUserInterfaceMessage
{
    public readonly string ItemId = itemId;
}

[Serializable, NetSerializable]
public sealed class CateringDigiboardEditItemMessage(string itemId, string name, string description, int scripCost)
    : BoundUserInterfaceMessage
{
    public readonly string ItemId = itemId;
    public readonly string Name = name;
    public readonly string Description = description;
    public readonly int ScripCost = scripCost;
}

[Serializable, NetSerializable]
public sealed class CateringDigiboardReorderMessage(string itemId, int newIndex) : BoundUserInterfaceMessage
{
    public readonly string ItemId = itemId;
    public readonly int NewIndex = newIndex;
}

[Serializable, NetSerializable]
public sealed class CateringDigiboardPrintMessage(int count) : BoundUserInterfaceMessage
{
    public readonly int Count = count;
}
