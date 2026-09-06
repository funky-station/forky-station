using System.Collections.Generic;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.Medical;

[Serializable, NetSerializable]
public enum BedInternalsVisuals
{
    TankVisual
}

[Serializable, NetSerializable]
public enum BedTankVisual
{
    None,
    Nitrogen,
    Oxygen,
    Generic,
    Plasma,
    Nitrous
}

[RegisterComponent, NetworkedComponent]
public sealed partial class BedInternalsComponent : Component
{
    [DataField("slot", required: true)]
    public string GasSlot = default!;

    [DataField("maskPrototype")]
    public string MaskPrototype = "ClothingMaskBreathMedical";

    public bool Enabled;
    public EntityUid? CachedTank;

    public Dictionary<EntityUid, EntityUid> TempMasks = new();
    public Dictionary<EntityUid, EntityUid> StoredMasks = new();
    public Dictionary<EntityUid, bool> OriginalInternalsState = new();
    public Dictionary<EntityUid, EntityUid?> OriginalGasTank = new();
    public Dictionary<EntityUid, bool> BedProvidedTank = new();
}
