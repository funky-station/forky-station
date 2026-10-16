using Content.Shared._Funkystation.SM;
using Content.Shared._Funkystation.SM.Components;

namespace Content.Server._Funkystation.SM.Events;
[ByRefEvent]
public readonly record struct SupermatterDelaminationEvent(
    EntityUid SupermatterUid,
    GasCharacteristicsType DominantCharacteristic,
    DelamType DelamType);

