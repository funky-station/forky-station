using Content.Shared._Funkystation.Cpr;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using System.Numerics;

namespace Content.Client._Funkystation.Cpr;

public sealed partial class CprSystem : SharedCprSystem
{
    [Dependency] private AnimationPlayerSystem _animation = null!;

    private const string LungeKey = "cpr-lunge";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<CprLungeEvent>(OnCprLunge);
    }

    /// <summary>
    /// Used for playing the animation of other entities doing CPR
    /// </summary>
    private void OnCprLunge(CprLungeEvent args)
    {
        var ent = GetEntity(args.Ent);
        if (Exists(ent))
            DoLunge(ent);
    }

    public override void DoLunge(EntityUid user)
    {
        if (!Timing.IsFirstTimePredicted)
            return;

        // play the CPR animation
        var lunge = GetLungeAnimation(new Vector2(0f, -1f)); // downwards vector, mimics the motion of pressing down on someone's chest
        _animation.Stop(user, LungeKey);
        _animation.Play(user, lunge, LungeKey);
    }

    private Animation GetLungeAnimation(Vector2 direction)
    {
        const float endLength = CprAnimationLength;

        var animationTrack = new AnimationTrackComponentProperty()
        {
            ComponentType = typeof(SpriteComponent),
            Property = nameof(SpriteComponent.Offset),
            InterpolationMode = AnimationInterpolationMode.Cubic,
            KeyFrames =
            {
                new AnimationTrackProperty.KeyFrame(new Vector2(0f, 0f), 0f),
                new AnimationTrackProperty.KeyFrame(direction.Normalized() * 0.12f, endLength * 0.2f),
                new AnimationTrackProperty.KeyFrame(direction.Normalized() * 0.16f, endLength * 0.4f),
                new AnimationTrackProperty.KeyFrame(new Vector2(0f, 0f), endLength)
            }
        };

        return new Animation
        {
            Length = TimeSpan.FromSeconds(CprAnimationEndTime),
            AnimationTracks =
            {
                animationTrack
            }
        };
    }
}
