using Content.Shared._Funkystation.Stains.Components;
using Content.Shared._Funkystation.Stains.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.Stains;

public sealed partial class StainSystem : SharedStainSystem
{
    [Dependency] private TagSystem _tag = null!;
    private readonly ProtoId<TagPrototype> _dnaSolutionScannable = "DNASolutionScannable";

    protected override void OnStained(Entity<StainableComponent> ent, Entity<SolutionComponent> solution)
    {
        base.OnStained(ent, solution);

        _tag.AddTag(ent.Owner, _dnaSolutionScannable);
    }
}
