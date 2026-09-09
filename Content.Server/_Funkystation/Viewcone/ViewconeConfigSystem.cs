using Content.Shared._ES.Viewcone.Components;
using Content.Shared._Funkystation.CCVar;
using Robust.Shared.Configuration;

namespace Content.Server._Funkystation.Viewcone;

// applies funkystation.viewcone_base_angle to all entities with a viewcone
public sealed partial class ViewconeConfigSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = null!;
    public override void Initialize()
    {
        base.Initialize();
        Subs.CVar(_cfg, ViewconeCCVars.ViewconeBaseAngle, OnChanged, invokeImmediately: true);
    }

    private void OnChanged(float angle)
    {
        var query = EntityQueryEnumerator<ESViewconeComponent>();
        while (query.MoveNext(out var uid, out var viewcone))
        {
            viewcone.BaseConeAngle = angle;
            Dirty(uid, viewcone);
        }
    }
}
