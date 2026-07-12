using System.Numerics;
using Content.Client._Funkystation.Emoting.UI;
using Content.Client.Gameplay;
using Content.Shared._Funkystation.Emoting;
using Content.Shared._Funkystation.Emoting.Components;
using Content.Shared._Funkystation.Emoting.EntitySystems;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Animations;
using Robust.Shared.Input.Binding;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client._Funkystation.Emoting;

[UsedImplicitly]
public sealed partial class DuoEmoteSystem : SharedDuoEmoteSystem
{
    [Dependency] private IEyeManager _eye = null!;
    [Dependency] private IInputManager _input = null!;
    [Dependency] private IPlayerManager _player = null!;
    [Dependency] private IStateManager _state = null!;
    [Dependency] private AnimationPlayerSystem _animation = null!;
    [Dependency] private EntityLookupSystem _lookup = null!;
    [Dependency] private SharedTransformSystem _xform = null!;
    [Dependency] private SpriteSystem _sprite = null!;
    [Dependency] private DuoEmoteAnimationSystem _emoteAnim = null!;

    private const string LungeAnimationKey = "duo-emote-lunge";

    private DuoEmoteMenu? _menu;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeNetworkEvent<DuoEmoteLungeEvent>(OnLungeEvent);
        SubscribeLocalEvent<DuoEmoteVisualsComponent, ComponentStartup>(OnVisualsStartup);
        SubscribeLocalEvent<DuoEmoteVisualsComponent, AfterAutoHandleStateEvent>(OnVisualsState);

        CommandBinds.Builder
            .Bind(ContentKeyFunctions.OpenEmotesMenu, new PointerInputCmdHandler(OpenEmoteMenuOrDuoMenu, outsidePrediction: false))
            .Register<DuoEmoteSystem>();
    }

    public override void Shutdown()
    {
        base.Shutdown();
        CommandBinds.Unregister<DuoEmoteSystem>();
        _menu?.Close();
    }

    private void OnVisualsStartup(Entity<DuoEmoteVisualsComponent> ent, ref ComponentStartup args)
    {
        UpdateVisuals(ent);
    }

    private void OnVisualsState(Entity<DuoEmoteVisualsComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        UpdateVisuals(ent);
    }

    private void UpdateVisuals(Entity<DuoEmoteVisualsComponent> ent)
    {
        // Sprite component and prototype
        if (TryComp<SpriteComponent>(ent.Owner, out var spriteComp) && _prototype.TryIndex(ent.Comp.EmoteId, out var proto))
        {
            var entity = (ent.Owner, spriteComp);
            const string layerKey = "emote";

            if (!_sprite.LayerMapTryGet(entity, layerKey, out _, false))
            {
                var index = _sprite.AddLayer(entity, proto.EffectSprite);
                _sprite.LayerMapSet(entity, layerKey, index);
            }

            _sprite.LayerSetSprite(entity, layerKey, proto.EffectSprite);
            spriteComp.LayerSetShader(LayerMapGet(entity, layerKey), "unshaded");
        }
    }

    private int LayerMapGet(Entity<SpriteComponent?> sprite, string key)
    {
        _sprite.LayerMapTryGet(sprite, key, out var index, false);
        return index;
    }

    private void OnLungeEvent(DuoEmoteLungeEvent ev)
    {
        var initiator = GetEntity(ev.Initiator);
        var partner = GetEntity(ev.Partner);

        // Base lunge animation
        PlayLunge(initiator, partner);
        PlayLunge(partner, initiator);

        // Trigger custom animation hook
        if (!string.IsNullOrEmpty(ev.Animation))
        {
            _emoteAnim.TryPlayAnimation(ev.Animation, initiator, partner);
        }
    }

    private void PlayLunge(EntityUid uid, EntityUid toward)
    {
        var myPos = _xform.GetMapCoordinates(uid).Position;
        var theirPos = _xform.GetMapCoordinates(toward).Position;
        var dir = theirPos - myPos;

        if (dir == Vector2.Zero)
            return;

        var localDir = dir.Normalized() * 0.15f;

        const float length = 0.1f;
        var anim = new Animation
        {
            Length = TimeSpan.FromSeconds(length),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Offset),
                    InterpolationMode = AnimationInterpolationMode.Linear,
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(Vector2.Zero, 0f),
                        new AnimationTrackProperty.KeyFrame(localDir, length * 0.4f),
                        new AnimationTrackProperty.KeyFrame(Vector2.Zero, length * 0.6f),
                    },
                },
            },
        };

        _animation.Stop(uid, LungeAnimationKey);
        _animation.Play(uid, anim, LungeAnimationKey);
    }

    private bool OpenEmoteMenuOrDuoMenu(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
    {
        if (_state.CurrentState is not GameplayState)
            return false;

        var player = _player.LocalEntity;
        if (player == null)
            return false;

        if (!TryComp<DuoEmoteComponent>(player, out var selfComp) || selfComp.Active)
            return false;

        var target = GetHoveredDuoEmoteTarget(player.Value);

        if (target == null)
            return false;

        // Return true to eat the input and not show the normal emote wheel
        OpenDuoEmoteMenu(player.Value, target.Value);
        return true;
    }

    private Entity<DuoEmoteComponent>? GetHoveredDuoEmoteTarget(EntityUid player)
    {
        if (!TryGetMapCoordinatesUnderCursor(out var mapCoords))
            return null;

        const float pickRadius = 0.5f;
        var nearby = new HashSet<Entity<DuoEmoteComponent>>();
        _lookup.GetEntitiesInRange(mapCoords, pickRadius, nearby);

        Entity<DuoEmoteComponent>? best = null;
        var bestDist = float.MaxValue;

        foreach (var candidate in nearby)
        {
            if (candidate.Owner == player)
                continue;

            if (!HasComp<DuoEmoteComponent>(candidate.Owner))
                continue;

            var dist = (_xform.GetMapCoordinates(candidate.Owner).Position - mapCoords.Position).Length();
            if (dist < bestDist)
            {
                bestDist = dist;
                best = candidate;
            }
        }

        return best;
    }

    private bool TryGetMapCoordinatesUnderCursor(out MapCoordinates mapCoords)
    {
        mapCoords = MapCoordinates.Nullspace;

        var mouseScreen = _input.MouseScreenPosition;
        if (!mouseScreen.IsValid)
            return false;

        mapCoords = _eye.ScreenToMap(mouseScreen);
        return mapCoords.MapId != MapId.Nullspace;
    }

    private void OpenDuoEmoteMenu(EntityUid player, Entity<DuoEmoteComponent> target)
    {
        _menu?.Close();
        _menu = new DuoEmoteMenu(player, target);

        _menu.OnEmoteSelected += emoteId =>
        {
            _menu?.Close();

            if (!TryComp<DuoEmoteComponent>(player, out _))
                return;

            RaisePredictiveEvent(new DuoEmoteAttemptEvent(GetNetEntity(target.Owner), emoteId));
        };

        _menu.OpenOverMouseScreenPosition();
    }
}
