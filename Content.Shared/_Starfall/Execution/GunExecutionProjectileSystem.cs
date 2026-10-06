using Content.Shared.Projectiles;
using Content.Shared.Trigger.Components.Triggers;
using Content.Shared.Trigger.Systems;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Network;

namespace Content.Shared._Starfall.Execution;

/// <summary>
/// Resolves ammo directly against an execution victim without firing it as a travelling projectile.
/// </summary>
public sealed partial class GunExecutionProjectileSystem : EntitySystem
{
    [Dependency] private TriggerSystem _trigger = default!;
    [Dependency] private INetManager _net = default!;
    [Dependency] private SharedTransformSystem _transform = default!;

    public EntityUid? MaterializeProjectile(EntityUid? ammoEntity, IShootable shootable, EntityUid victim)
    {
        if (!_net.IsServer)
            return null;

        return shootable switch
        {
            CartridgeAmmoComponent cartridge => Spawn(cartridge.Prototype, Transform(victim).Coordinates),

            AmmoComponent when ammoEntity != null => MoveProjectile(ammoEntity.Value, victim),

            _ => null,
        };
    }

    public void ResolveProjectile(EntityUid projectile, EntityUid victim, EntityUid attacker)
    {
        if (!_net.IsServer)
            return;

        if (TryComp<ProjectileComponent>(projectile, out var projectileComponent))
        {
            projectileComponent.Shooter = attacker;
            Dirty(projectile, projectileComponent);

            var hit = new ProjectileHitEvent(projectileComponent.Damage, victim, attacker);

            RaiseLocalEvent(projectile, ref hit);
        }

        if (HasComp<TriggerOnCollideComponent>(projectile))
            _trigger.Trigger(projectile, attacker);

        if (!TerminatingOrDeleted(projectile))
            QueueDel(projectile);
    }

    // I shit you not, we teleport the projectile into the victim and resolve the hit immediately.
    private EntityUid MoveProjectile(EntityUid projectile, EntityUid victim)
    {
        var xform = Transform(projectile);
        _transform.SetCoordinates(projectile, xform, Transform(victim).Coordinates);
        return projectile;
    }
}
