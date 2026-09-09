using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;

namespace Content.Client._Funkystation.Emoting;

/// <summary>
/// Handles custom animations for duo emotes that play in addition to the lunge
/// </summary>
public sealed partial class DuoEmoteAnimationSystem : EntitySystem
{
    [Dependency] private AnimationPlayerSystem _animation = null!;

    private const string CustomAnimationKey = "duo-emote-custom";

    private readonly Dictionary<string, Action<EntityUid, EntityUid>> _registry = new();

    public override void Initialize()
    {
        base.Initialize();

        // Register animations here
        RegisterAnimation("Spin", PlaySpinAnimation);
    }

    /// <summary>
    /// Allows other systems to register their own animations
    /// </summary>
    private void RegisterAnimation(string id, Action<EntityUid, EntityUid> handler)
    {
        _registry[id] = handler;
    }

    /// <summary>
    /// Play animation on both participants
    /// </summary>
    public bool TryPlayAnimation(string id, EntityUid initiator, EntityUid partner)
    {
        if (!_registry.TryGetValue(id, out var handler))
            return false;

        handler(initiator, partner);
        handler(partner, initiator);

        return true;
    }

    // Animations will be defined down here.

    // 360° Spin.
    private void PlaySpinAnimation(EntityUid uid, EntityUid partner)
    {
        if (!TryComp(uid, out TransformComponent? xform))
            return;

        var startRot = xform.LocalRotation;
        const float length = 0.4f;

        var anim = new Animation
        {
            Length = TimeSpan.FromSeconds(length),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
                {
                    ComponentType = typeof(TransformComponent),
                    Property = nameof(TransformComponent.LocalRotation),
                    InterpolationMode = AnimationInterpolationMode.Linear,
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(startRot, 0f),
                        new AnimationTrackProperty.KeyFrame(startRot + new Angle(Math.PI / 2), length * 0.25f),
                        new AnimationTrackProperty.KeyFrame(startRot + new Angle(Math.PI), length * 0.5f),
                        new AnimationTrackProperty.KeyFrame(startRot + new Angle(Math.PI * 1.5), length * 0.75f),
                        new AnimationTrackProperty.KeyFrame(startRot, length),
                    }
                }
            }
        };

        _animation.Stop(uid, CustomAnimationKey);
        _animation.Play(uid, anim, CustomAnimationKey);
    }
}
