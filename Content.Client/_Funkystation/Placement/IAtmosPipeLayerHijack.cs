using Content.Shared.Construction.Prototypes;
using Robust.Client.Placement;

namespace Content.Client._Funkystation.Placement;

// funky. so AlignAtmosPipeLayers layer switching can rebuild whatever hijack is currently active
public interface IAtmosPipeLayerHijack
{
    ConstructionPrototype? CurrentPrototype { get; }

    /// <summary>
    /// returns a new hijack of the same type
    /// </summary>
    PlacementHijack WithPrototype(ConstructionPrototype newPrototype);
}
