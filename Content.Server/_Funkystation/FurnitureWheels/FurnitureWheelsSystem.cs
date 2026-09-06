using Content.Shared._Funkystation.FurnitureWheels;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Utility;

namespace Content.Server._Funkystation.FurnitureWheels;

public sealed partial class FurnitureWheelsSystem : EntitySystem
{
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private PullingSystem _pulling = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<FurnitureWheelsComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
        SubscribeLocalEvent<FurnitureWheelsComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<FurnitureWheelsComponent, AnchorStateChangedEvent>(OnAnchorChanged);
    }

    private void OnMapInit(EntityUid uid, FurnitureWheelsComponent comp, MapInitEvent args)
    {
        // Sync the visualizer to whatever state the entity spawned in.
        _appearance.SetData(uid, FurnitureWheelsVisuals.Locked, Transform(uid).Anchored);
    }

    private void OnGetVerbs(
        EntityUid uid,
        FurnitureWheelsComponent comp,
        GetVerbsEvent<InteractionVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        var locked = Transform(uid).Anchored;
        var user = args.User;

        args.Verbs.Add(new InteractionVerb
        {
            Text = locked ? "Unlock wheels" : "Lock wheels",
            Icon = new SpriteSpecifier.Texture(new(locked
                ? "/Textures/Interface/VerbIcons/unlock.svg.192dpi.png"
                : "/Textures/Interface/VerbIcons/lock.svg.192dpi.png")),
            Act = () => ToggleWheels(uid, comp, user)
        });
    }

    private void ToggleWheels(EntityUid uid, FurnitureWheelsComponent comp, EntityUid user)
    {
        var xform = Transform(uid);
        var willBeLocked = !xform.Anchored;

        if (xform.Anchored)
        {
            _transform.Unanchor(uid, xform);
        }
        else
        {
            // About to lock — stop anyone dragging it first, otherwise anchoring fights the pull.
            if (TryComp<PullableComponent>(uid, out var pullable))
                _pulling.TryStopPull(uid, pullable, user);

            _transform.AnchorEntity(uid, xform);
        }

        var sound = willBeLocked ? comp.LockSound : comp.UnlockSound;
        if (sound != null)
            _audio.PlayPvs(sound, uid);
    }

    private void OnAnchorChanged(EntityUid uid, FurnitureWheelsComponent comp, ref AnchorStateChangedEvent args)
    {
        // Just keep the visualizer honest. Sound is intentionally NOT played here:
        // other anchor paths (wrench, admin verbs, construction) have their own audio
        // and we don't want to double up.
        if (args.Detaching)
            return;

        _appearance.SetData(uid, FurnitureWheelsVisuals.Locked, args.Anchored);
    }
}
