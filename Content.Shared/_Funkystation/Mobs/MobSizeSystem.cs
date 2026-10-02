using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.Mobs;

public sealed class MobSizeSystem : EntitySystem
{

    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<MobSizeComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, MobSizeComponent comp, ComponentStartup args)
    {
        if (_proto.TryIndex(comp.SizeId, out MobSizePrototype? proto))
            comp.SizeProto = proto;
    }
}
