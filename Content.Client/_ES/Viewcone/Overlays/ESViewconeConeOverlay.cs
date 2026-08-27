using Content.Client.Eye;
using Content.Shared._ES.Viewcone;
using Content.Shared._ES.Viewcone.Components;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Client._ES.Viewcone.Overlays;

/// <summary>
///     Renders the actual "cone" part of the viewcone, no alpha modulation
/// </summary>
public sealed partial class ESViewconeConeOverlay : Overlay
{
    [Dependency] private IEntityManager _ent = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    private readonly ESViewconeAngleSystem _angle;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;

    public static ProtoId<ShaderPrototype> ShaderPrototype = "Viewcone";
    private readonly ShaderInstance _viewconeShader;

    private Entity<EyeComponent, ESViewconeComponent, TransformComponent>? _eyeEntity;
    private float _coneAngle;
    private float _coneFeather;
    private float _coneIgnoreRadius;
    private float _coneIgnoreFeather;

    public ESViewconeConeOverlay()
    {
        IoCManager.InjectDependencies(this);

        _angle = _ent.EntitySysManager.GetEntitySystem<ESViewconeAngleSystem>();

        _viewconeShader = _proto.Index(ShaderPrototype).InstanceUnique();
        ZIndex = -6;
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        _eyeEntity = null;

        // This is really stupid but there isn't another way to reverse an eye entity from just an IEye afaict
        // It's not really inefficient though. theres barely any of those fuckin things anyway
        // lerpingeye used because that system already does the busywork of figuring out which eyes are 'rendering' sort of
        // so we dont have to query other players eyes (probably barely makes a difference anyway)
        var enumerator = _ent.AllEntityQueryEnumerator<LerpingEyeComponent, EyeComponent, ESViewconeComponent, TransformComponent>();
        while (enumerator.MoveNext(out var uid, out var _, out var eye, out var viewcone, out var xform))
        {
            if (args.Viewport.Eye != eye.Eye)
                continue;

            // todo dont really like that this has to get the angle twice (once here and once in the alpha overlay)
            // but its not really like its a huge inefficiency (this only has to happen twice per frame and its like a trivial event relay with no logic)
            // and i really dont want to make it stateful
            _coneAngle = _angle.GetModifiedViewconeAngle((uid, viewcone));
            _coneFeather = _coneAngle <= 0f ? 0.01f : viewcone.ConeFeather; // semi-hack to make 0-angle viewcone look correct
            _coneIgnoreRadius = (viewcone.ConeIgnoreRadius - viewcone.ConeIgnoreFeather) * 50f;
            _coneIgnoreFeather = Math.Max(viewcone.ConeIgnoreFeather * 200f, 8f);
            _eyeEntity = (uid, eye, viewcone, xform);
            break;
        }

        return _eyeEntity != null;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null || _eyeEntity == null)
            return;

        var worldHandle = args.WorldHandle;
        var viewport = args.WorldBounds;

        _viewconeShader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _viewconeShader.SetParameter("Zoom", _eyeEntity.Value.Comp1.Zoom.X);
        _viewconeShader.SetParameter("ViewAngle", (float) _eyeEntity.Value.Comp2.ViewAngle.Theta);
        _viewconeShader.SetParameter("ConeAngle", _coneAngle);
        _viewconeShader.SetParameter("ConeFeather", _coneFeather);
        _viewconeShader.SetParameter("ConeIgnoreRadius", _coneIgnoreRadius);
        _viewconeShader.SetParameter("ConeIgnoreFeather", _coneIgnoreFeather);

        worldHandle.UseShader(_viewconeShader);
        worldHandle.DrawRect(viewport, Color.White);
        worldHandle.UseShader(null);
        _eyeEntity = null;
    }
}
