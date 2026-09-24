using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Jittering;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._Funkystation.Traits.Migraines;

/// <summary>
/// Handles effects when a migraine status effect is applied to an entity.
/// </summary>
public sealed partial class MigraineSystem : EntitySystem
{
    private static readonly SoundSpecifier MigraineSound = new SoundPathSpecifier("/Audio/_Funkystation/Effects/migraine.ogg");

    [Dependency] private MovementSpeedModifierSystem _movementSpeed = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedJitteringSystem _jittering = null!;
    [Dependency] private INetManager _net = null!;
    [Dependency] private IGameTiming _timing = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MigraineEffectComponent, StatusEffectAppliedEvent>(OnMigraineApplied);
        SubscribeLocalEvent<MigraineEffectComponent, StatusEffectRemovedEvent>(OnMigraineRemoved);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_net.IsServer)
            return;

        var query = EntityQueryEnumerator<MigraineEffectComponent, StatusEffectComponent>();
    }

    private void OnMigraineApplied(Entity<MigraineEffectComponent> entity, ref StatusEffectAppliedEvent args)
    {
        _movementSpeed.RefreshMovementSpeedModifiers(args.Target);

        if (_net.IsServer)
            _jittering.AddJitter(args.Target, -85f, 2f);

        if (_net.IsServer)
            _audio.PlayEntity(MigraineSound, args.Target, args.Target);

        if (!_net.IsServer)
            return;

        if (!entity.Comp.ShowPopup)
            return;

        var popupEvent = new MigrainePopupEvent(entity.Comp.SelfPopup, entity.Comp.OthersPopup);

        // Other systems can modify or cancel the popup.
        RaiseLocalEvent(args.Target, popupEvent);

        if (popupEvent.Cancelled)
            return;

        if (popupEvent.SelfPopup != null)
            _popup.PopupEntity(Loc.GetString(popupEvent.SelfPopup), args.Target, args.Target, PopupType.MediumCaution);

        if (popupEvent.OthersPopup != null)
            _popup.PopupEntity(Loc.GetString(popupEvent.OthersPopup, ("target", args.Target)), args.Target, Filter.PvsExcept(args.Target), true, PopupType.SmallCaution);
    }

    private void OnMigraineRemoved(Entity<MigraineEffectComponent> entity, ref StatusEffectRemovedEvent args)
    {
        _movementSpeed.RefreshMovementSpeedModifiers(args.Target);

        if (!_net.IsServer)
            return;

        RemCompDeferred<JitteringComponent>(args.Target);
    }
}
