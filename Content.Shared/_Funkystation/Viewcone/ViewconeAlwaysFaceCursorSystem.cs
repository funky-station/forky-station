using Content.Shared._Funkystation.CCVar;
using Content.Shared.CombatMode;
using Content.Shared._ES.Viewcone.Components;
using Content.Shared.MouseRotator;
using Content.Shared.Movement.Components;
using Robust.Shared.Configuration;
using Robust.Shared.Network;

namespace Content.Shared._Funkystation.Viewcone;

// controls whether mobs always rotate to face the mouse cursor (funkystation.always_face_cursor)
public sealed partial class ViewconeAlwaysFaceCursorSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private INetManager _net = null!;

    private bool _enabled;
    private float _baseAngle;

    public override void Initialize()
    {
        base.Initialize();

        if (_net.IsClient)
            return;

        SubscribeLocalEvent<ESViewconeComponent, ComponentInit>(OnMobInit);
        SubscribeLocalEvent<CombatModeComponent, ComponentInit>(OnCombatModeInit);

        Subs.CVar(_cfg, ViewconeCCVars.AlwaysFaceCursor, OnChanged, invokeImmediately: true);
        Subs.CVar(_cfg, ViewconeCCVars.ViewconeBaseAngle, angle => _baseAngle = angle, invokeImmediately: true);
    }

    private void OnMobInit(Entity<ESViewconeComponent> ent, ref ComponentInit args)
    {
        ApplyToMob(ent.Owner, _enabled);
        ent.Comp.BaseConeAngle = _baseAngle;
        Dirty(ent);
    }

    private void OnCombatModeInit(Entity<CombatModeComponent> ent, ref ComponentInit args)
    {
        if (_enabled)
        {
            ent.Comp.ToggleMouseRotator = false;
            Dirty(ent);
        }
    }

    private void OnChanged(bool enabled)
    {
        _enabled = enabled;

        var mobQuery = EntityQueryEnumerator<ESViewconeComponent>();
        while (mobQuery.MoveNext(out var uid, out _))
        {
            ApplyToMob(uid, enabled);
        }

        if (enabled)
        {
            var combatQuery = EntityQueryEnumerator<CombatModeComponent>();
            while (combatQuery.MoveNext(out var uid, out var combatMode))
            {
                combatMode.ToggleMouseRotator = false;
                Dirty(uid, combatMode);
            }
        }
    }

    private void ApplyToMob(EntityUid uid, bool enabled)
    {
        if (enabled)
        {
            EnsureComp<MouseRotatorComponent>(uid);
            EnsureComp<NoRotateOnMoveComponent>(uid);
        }
        else
        {
            RemComp<MouseRotatorComponent>(uid);
            RemComp<NoRotateOnMoveComponent>(uid);
        }
    }
}
