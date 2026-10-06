using System.Numerics;
using Content.Shared.Camera;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._Starfall.Weapons;

/// <summary>
/// Plays gun firing effects without consuming ammunition or launching a projectile.
/// </summary>
/// <remarks>
/// This is basically only used for <see cref="GunExecutionSystem"/> but I wanted to keep it separate cause like.
/// What if someone wants to use it for something else? I dunno! Get atomized.
/// </remarks>
public sealed partial class SfGunEffectsSystem : EntitySystem
{
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedCameraRecoilSystem _recoil = default!;
    [Dependency] private INetManager _net = default!;
    [Dependency] private IGameTiming _timing = default!;

    /// <summary>
    /// Plays muzzle flash, firing sound, and predicted camera recoil.
    /// </summary>
    public void PlayGunEffects(Entity<GunComponent> gun, IShootable shootable, EntityUid? user, Vector2 direction)
    {
        var normalizedDirection = direction != Vector2.Zero ? direction.Normalized() : Vector2.Zero;

        PlayMuzzleFlash(gun, shootable, user, normalizedDirection.ToAngle());
        PlayRecoil(gun, user, -normalizedDirection);
        PlayGunshotSound(gun, user);
    }

    private void PlayMuzzleFlash(Entity<GunComponent> gun, IShootable shootable, EntityUid? user, Angle angle)
    {
        if (shootable is not AmmoComponent
            {
                MuzzleFlash: { } muzzlePrototype
            })
        {
            return;
        }

        var attempt = new GunMuzzleFlashAttemptEvent();
        RaiseLocalEvent(gun, ref attempt);

        if (attempt.Cancelled)
            return;

        var flash = new MuzzleFlashEvent(GetNetEntity(gun), muzzlePrototype, angle);

        RaiseLocalEvent(flash);

        if (!_net.IsServer)
            return;

        var filter = Filter.Pvs(gun, entityManager: EntityManager);

        if (user != null)
            filter.RemovePlayerByAttachedEntity(user.Value);

        RaiseNetworkEvent(flash, filter);
    }

    private void PlayRecoil(Entity<GunComponent> gun, EntityUid? user, Vector2 direction)
    {
        if (!_net.IsClient ||
            !_timing.IsFirstTimePredicted ||
            user == null ||
            direction == Vector2.Zero)
        {
            return;
        }

        var recoilScalar = gun.Comp.CameraRecoilScalarModified;

        if (recoilScalar == 0f)
            return;

        _recoil.KickCamera(
            user.Value,
            direction * 0.5f * recoilScalar);
    }

    private void PlayGunshotSound(Entity<GunComponent> gun, EntityUid? user)
    {
        _audio.PlayPredicted(gun.Comp.SoundGunshotModified ?? gun.Comp.SoundGunshot, gun, user);
    }
}
