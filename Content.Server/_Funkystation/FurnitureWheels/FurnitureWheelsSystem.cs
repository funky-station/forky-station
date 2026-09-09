using Content.Shared._Funkystation.FurnitureWheels;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Server._Funkystation.FurnitureWheels;

public sealed partial class FurnitureWheelsSystem : EntitySystem
{
    [Dependency] private SharedTransformSystem _transform = null!;
    [Dependency] private PullingSystem _pulling = null!;

    public override void Initialize()
    {
        SubscribeLocalEvent<FurnitureWheelsComponent, GetVerbsEvent<InteractionVerb>>(OnGetVerbs);
    }

    private void OnGetVerbs(
        EntityUid uid,
        FurnitureWheelsComponent comp,
        GetVerbsEvent<InteractionVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        var verb = new InteractionVerb
        {
            Text = comp.Locked ? "Unlock wheels" : "Lock wheels",
            Icon = new SpriteSpecifier.Texture(new (comp.Locked ? "/Textures/Interface/VerbIcons/unlock.svg.192dpi.png" : "/Textures/Interface/VerbIcons/lock.svg.192dpi.png")),
            Act = () =>
            {
                comp.Locked = !comp.Locked;

                var xform = Transform(uid);

                if (comp.Locked)
                {
                    if (TryComp<PullableComponent>(uid, out var pullable))
                    {
                        _pulling.TryStopPull(uid, pullable, args.User);
                    }

                    _transform.AnchorEntity(uid, xform);
                }
                else
                {
                    _transform.Unanchor(uid, xform);
                }
            }
        };

        args.Verbs.Add(verb);
    }
}
