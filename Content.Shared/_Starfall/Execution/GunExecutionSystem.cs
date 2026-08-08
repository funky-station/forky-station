using System.Linq;
using Content.Shared._Starfall.Weapons;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Execution;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Hitscan.Components;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starfall.Execution;

/// <summary>
/// Handles the gun execution verb system.
/// </summary>
/// <remarks>
/// Guns that should not be able to execute should have <see cref="GunExecutionBlacklistComponent"/>.
/// </remarks>
public sealed partial class GunExecutionSystem : EntitySystem
{
    [Dependency] private SharedExecutionSystem _execution = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;
    [Dependency] private GunExecutionProjectileSystem _projectiles = default!;
    [Dependency] private SfGunEffectsSystem _gunEffects = default!;
    [Dependency] private SharedSuicideSystem _suicide = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private IComponentFactory _compFactory = default!;
    [Dependency] private INetManager _net = default!;
    [Dependency] private EntityWhitelistSystem _whitelist = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GunComponent, GetVerbsEvent<UtilityVerb>>(OnGetVerbs);
        SubscribeLocalEvent<GunComponent, GunExecutionDoAfterEvent>(OnDoAfter);
    }

    private void OnGetVerbs(Entity<GunComponent> ent, ref GetVerbsEvent<UtilityVerb> args)
    {
        // Is this gun blacklisted from executions?
        if (HasComp<GunExecutionBlacklistComponent>(ent))
            return;

        // Who is the attacker and who is the victim?
        var attacker = args.User;
        var victim = args.Target;
        var weapon = ent.Owner;

        // Can the victim be executed by the attacker?
        if (!_execution.CanBeExecuted(victim, attacker))
            return;

        // Grab execution time from the gun's execution component if it has one, otherwise use the default execution time.
        var executionTime = TryComp<GunExecutionComponent>(ent, out var cfg) ? cfg.ExecutionTime : GunExecutionComponent.DefaultExecutionTime;

        // Add the execution verb to the list of verbs.
        args.Verbs.Add(new UtilityVerb
        {
            Act = () => TryBeginExecution(weapon, victim, attacker, executionTime),
            Impact = LogImpact.High,
            Text = Loc.GetString("execution-verb-name"),
            Message = Loc.GetString("execution-verb-message"),
        });
    }

    private void TryBeginExecution(EntityUid weapon, EntityUid victim, EntityUid attacker, TimeSpan executionTime)
    {
        // Check if the victim can be executed by the attacker again, since the state may have changed since the verb was added.
        if (!_execution.CanBeExecuted(victim, attacker))
            return;

        // Are we executing ourselves or someone else?
        if (attacker == victim)
        {
            ShowInternal("gun-execution-suicide-initial-self", attacker, victim, weapon);
            ShowExternal("gun-execution-suicide-initial-others", attacker, victim, weapon);
        }
        else
        {
            ShowInternal("gun-execution-initial-self", attacker, victim, weapon);
            ShowExternal("gun-execution-initial-others", attacker, victim, weapon);
        }

        // Begin doAfter
        _doAfter.TryStartDoAfter(new DoAfterArgs(
            EntityManager,
            attacker,
            executionTime,
            new GunExecutionDoAfterEvent(),
            weapon,
            target: victim,
            used: weapon)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
        });
    }

    /// <summary>
    /// Completes a gun execution after the execution doafter finishes.
    /// </summary>
    /// <remarks>
    /// Consumes a round, plays audiovisual effects, and applies lethal damage to the target if the weapon has a damage type.
    /// </remarks>
    private void OnDoAfter(Entity<GunComponent> ent, ref GunExecutionDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled || args.Used == null || args.Target == null)
        {
            return;
        }

        var attacker = args.User;
        var victim = args.Target.Value;
        var weapon = args.Used.Value;

        if (!_execution.CanBeExecuted(victim, attacker))
            return;

        if (!TryComp<DamageableComponent>(victim, out var damageable))
            return;

        var takeAmmo = new TakeAmmoEvent(1, new List<(EntityUid? Entity, IShootable Shootable)>(), Transform(attacker).Coordinates, attacker);

        RaiseLocalEvent(weapon, takeAmmo);

        if (takeAmmo.Ammo.Count == 0)
        {
            _audio.PlayPredicted(ent.Comp.SoundEmpty, weapon, attacker);
            ShowInternal("gun-execution-empty-self", attacker, victim, weapon);
            ShowExternal("gun-execution-empty-others", attacker, victim, weapon);

            return;
        }

        var damageType = GetExecutionDamageType(takeAmmo.Ammo[0]);

        if (damageType == null &&
            attacker == victim &&
            TryComp<GunExecutionComponent>(weapon, out var executionConfig) &&
            executionConfig.SelfExecutionDamageType is { } selfDamageType &&
            (executionConfig.SelfExecutionUserWhitelist == null ||
             _whitelist.IsWhitelistPass(
                 executionConfig.SelfExecutionUserWhitelist,
                 attacker)))
        {
            damageType = selfDamageType;
        }

        var (ammoEntity, shootable) = takeAmmo.Ammo[0];

        var direction = _transform.GetWorldPosition(victim) - _transform.GetWorldPosition(attacker);

        // Play muzzle flash, recoil, and gunshot sound. This does not launch a projectile.
        _gunEffects.PlayGunEffects(ent, shootable, attacker, direction);

        var projectile = _projectiles.MaterializeProjectile(ammoEntity, shootable, victim);

        if (projectile != null)
            _projectiles.ResolveProjectile(projectile.Value, victim, attacker);

        ConsumeExecutionAmmo(ammoEntity, shootable);

        // Notify systems that react to a gun being fired.
        var shotEvent = new GunShotEvent(attacker, takeAmmo.Ammo);
        RaiseLocalEvent(weapon, ref shotEvent);

        if (attacker == victim)
        {
            ShowInternal("gun-execution-suicide-complete-self", attacker, victim, weapon);
            ShowExternal("gun-execution-suicide-complete-others", attacker, victim, weapon);
        }
        else
        {
            ShowInternal("gun-execution-complete-self", attacker, victim, weapon);
            ShowExternal("gun-execution-complete-others", attacker, victim, weapon);
        }

        // Projectile payloads resolve normally, while execution damage guarantees that the victim is killed.
        if (damageType != null)
        {
            _suicide.ApplyLethalDamage((victim, damageable), new ProtoId<DamageTypePrototype>(damageType));
        }

        args.Handled = true;
    }

    private void ConsumeExecutionAmmo(EntityUid? ammoEntity, IShootable shootable)
    {
        switch (shootable)
        {
            case CartridgeAmmoComponent cartridge:
            {
                if (cartridge.DeleteOnSpawn)
                {
                    CleanupAmmoEntity(ammoEntity);
                    return;
                }

                if (ammoEntity == null)
                    return;

                cartridge.Spent = true;

                _appearance.SetData(ammoEntity.Value, AmmoVisuals.Spent, true);

                Dirty(ammoEntity.Value, cartridge);
                break;
            }

            case HitscanAmmoComponent:
                CleanupAmmoEntity(ammoEntity);
                break;

            case AmmoComponent:
                // On the server, GunExecutionProjectileSystem owns this entity
                // and deletes it after resolving its payload.
                if (!_net.IsServer)
                    CleanupAmmoEntity(ammoEntity);

                break;
        }
    }

    private void CleanupAmmoEntity(EntityUid? ammoEntity)
    {
        if (ammoEntity == null)
            return;

        if (IsClientSide(ammoEntity.Value))
            Del(ammoEntity.Value);
        else if (_net.IsServer)
            Del(ammoEntity.Value);
    }

    private string? GetExecutionDamageType(
        (EntityUid? Entity, IShootable Shootable) ammo)
    {
        var (ammoEntity, shootable) = ammo;

        switch (shootable)
        {
            case CartridgeAmmoComponent cartridge:
            {
                if (!_proto.TryIndex<EntityPrototype>(cartridge.Prototype, out var projectilePrototype))
                {
                    return null;
                }

                if (!projectilePrototype.TryGetComponent<ProjectileComponent>(out var projectile, _compFactory))
                {
                    return null;
                }

                return DominantDamageType(projectile.Damage);
            }

            case AmmoComponent:
            {
                if (ammoEntity == null || !TryComp<ProjectileComponent>(ammoEntity.Value, out var projectile))
                {
                    return null;
                }

                return DominantDamageType(projectile.Damage);
            }

            case HitscanAmmoComponent:
            {
                if (ammoEntity == null || !TryComp<HitscanBasicDamageComponent>(ammoEntity.Value, out var hitscanDamage))
                {
                    return null;
                }

                return DominantDamageType(hitscanDamage.Damage);
            }

            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the damage type key with the highest value
    /// </summary>
    private static string? DominantDamageType(DamageSpecifier damage)
    {
        foreach (var (damageType, amount) in damage.DamageDict
                     .OrderByDescending(entry => entry.Value))
        {
            if (amount <= 0)
                continue;

            if (string.Equals(damageType, "Structural", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            return damageType;
        }
        return null;
    }

    private void ShowInternal(LocId key, EntityUid attacker, EntityUid victim, EntityUid weapon)
    {
        _popup.PopupClient(
            Loc.GetString(key,
                ("attacker", Identity.Entity(attacker, EntityManager)),
                ("victim", Identity.Entity(victim, EntityManager)),
                ("weapon", weapon)),
            attacker,
            attacker,
            PopupType.MediumCaution);
    }

    private void ShowExternal(LocId key, EntityUid attacker, EntityUid victim, EntityUid weapon)
    {
        _popup.PopupEntity(
            Loc.GetString(key,
                ("attacker", Identity.Entity(attacker, EntityManager)),
                ("victim", Identity.Entity(victim, EntityManager)),
                ("weapon", weapon)),
            attacker,
            Filter.PvsExcept(attacker),
            true,
            PopupType.MediumCaution);
    }
}

