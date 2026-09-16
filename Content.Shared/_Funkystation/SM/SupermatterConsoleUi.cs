using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._Funkystation.SM;

[Serializable, NetSerializable]
public enum SupermatterConsoleUiKey : byte
{
    Key,
}

/// <summary>
/// Server-built state for the supermatter monitoring console: one "active" crystal on the same grid.
/// </summary>
[Serializable, NetSerializable]
public sealed class SupermatterConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
    public bool HasActiveCrystal;
    public NetEntity Crystal;
    public float IntegrityPercent;
    public float Integrity;
    public float MaxIntegrity;
    /// <summary>TG wiki internal energy (MeV/cm³); same stored value as server <c>SupermatterComponent.Power</c>.</summary>
    public float InternalEnergyMeV;
    public bool Delamming;
    /// <summary>Total moles in last absorbed mix (wiki singularity / cascade predicates).</summary>
    public float AbsorbedTotalMoles;
    /// <summary><see cref="DelamType"/> that would win if the crystal delaminated now (wiki priority).</summary>
    public byte PredictedDelamType;
    /// <summary>Seconds remaining if <see cref="Delamming"/>; otherwise 0.</summary>
    public float DelamCountdown;

    public SupermatterConsoleBoundUserInterfaceState(
        bool hasActiveCrystal,
        NetEntity crystal,
        float integrityPercent,
        float integrity,
        float maxIntegrity,
        float internalEnergyMeV,
        bool delamming,
        float absorbedTotalMoles,
        byte predictedDelamType,
        float delamCountdown)
    {
        HasActiveCrystal = hasActiveCrystal;
        Crystal = crystal;
        IntegrityPercent = integrityPercent;
        Integrity = integrity;
        MaxIntegrity = maxIntegrity;
        InternalEnergyMeV = internalEnergyMeV;
        Delamming = delamming;
        AbsorbedTotalMoles = absorbedTotalMoles;
        PredictedDelamType = predictedDelamType;
        DelamCountdown = delamCountdown;
    }
}
