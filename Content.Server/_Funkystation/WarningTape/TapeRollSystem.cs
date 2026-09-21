using System.Numerics;
using Content.Shared._Funkystation.CCVar;
using Content.Shared._Funkystation.WarningTape.Components;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Systems;

namespace Content.Server._Funkystation.WarningTape;

public sealed partial class TapeRollSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = null!;
    [Dependency] private SharedTransformSystem _transform = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private FixtureSystem _fixture = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TapeRollComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<TapeRollComponent, GotUnequippedHandEvent>(OnUnequipped);
        SubscribeLocalEvent<TapeRollComponent, DroppedEvent>(OnDropped);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var maxTiles = _cfg.GetCVar(WarningTapeCVars.MaxTiles);
        var rolls = EntityQueryEnumerator<TapeRollComponent, TransformComponent>();

        while (rolls.MoveNext(out var uid, out var roll, out _))
        {
            if (roll.PendingStart is not { } start || roll.PendingUser is not { } user)
                continue;

            if (!Exists(user))
            {
                roll.PendingStart = null;
                roll.PendingUser = null;
                Dirty(uid, roll);
                continue;
            }

            // check distance from player to anchor point
            var startMap = _transform.ToMapCoordinates(start);
            var userMap = _transform.GetMapCoordinates(user);

            var maxDistance = maxTiles * roll.TileLength;

            if (startMap.MapId == userMap.MapId && !((userMap.Position - startMap.Position).Length() > maxDistance))
                continue;

            // snap the tape
            roll.PendingStart = null;
            roll.PendingUser = null;
            Dirty(uid, roll);

            _popup.PopupEntity(Loc.GetString("warning-tape-roll-snapped"), user, user, PopupType.MediumCaution);
            _audio.PlayPvs(roll.SnapSound, user);
        }
    }

    private void OnUnequipped(Entity<TapeRollComponent> ent, ref GotUnequippedHandEvent args)
    {
        CancelPlacement(ent);
    }

    private void OnDropped(Entity<TapeRollComponent> ent, ref DroppedEvent args)
    {
        CancelPlacement(ent);
    }

    private void CancelPlacement(Entity<TapeRollComponent> ent)
    {
        if (ent.Comp.PendingStart == null)
            return;

        ent.Comp.PendingStart = null;
        ent.Comp.PendingUser = null;
        Dirty(ent);
    }

    private void OnAfterInteract(Entity<TapeRollComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Handled || !args.CanReach) // don't place tape from across the room bro :sob:
            return;

        var userMap = _transform.GetMapCoordinates(args.User);
        var clickMap = _transform.ToMapCoordinates(args.ClickLocation);

        if (userMap.MapId != clickMap.MapId || (userMap.Position - clickMap.Position).Length() > 1.0f)
            return;

        var roll = ent.Comp;
        args.Handled = true;

        if (roll.PendingStart is not { } start)
        {
            roll.PendingStart = args.ClickLocation;
            roll.PendingUser = args.User;
            Dirty(ent);
            _popup.PopupEntity(Loc.GetString("warning-tape-roll-anchored"), args.User, args.User);
            return;
        }

        // clear the pending point after this click
        roll.PendingStart = null;
        roll.PendingUser = null;
        Dirty(ent);

        var startMap = _transform.ToMapCoordinates(start);
        var endMap = _transform.ToMapCoordinates(args.ClickLocation);

        if (startMap.MapId != endMap.MapId)
            return;

        var rawDiff = endMap.Position - startMap.Position;
        var rawLength = rawDiff.Length();
        var maxTiles = _cfg.GetCVar(WarningTapeCVars.MaxTiles);
        var rawTiles = rawLength / roll.TileLength;

        if (rawTiles > maxTiles)
        {
            _popup.PopupEntity(Loc.GetString("warning-tape-roll-too-far"), args.User, args.User);
            return;
        }

        var tileCount = Math.Clamp((int) MathF.Round(rawTiles), 1, maxTiles);
        var direction = rawLength > 0.01f ? rawDiff / rawLength : new Vector2(1, 0);
        var snappedEnd = startMap.Position + direction * tileCount * roll.TileLength;
        var endCoords = _transform.ToCoordinates(new MapCoordinates(snappedEnd, startMap.MapId));

        var angle = new Angle(Math.Atan2(direction.Y, direction.X));

        var tape = Spawn(roll.TapePrototype, start);

        // rotate the entity so its X axis aligns with the line
        _transform.SetWorldRotation(tape, angle);

        var visuals = EnsureComp<TapeVisualsComponent>(tape);
        visuals.End = endCoords;
        visuals.TileCount = tileCount;
        Dirty(tape, visuals);

        // non-colliding fixture spanning from the origin to the end along the X axis
        var totalLength = tileCount * roll.TileLength;
        var shape = new PolygonShape();
        shape.SetAsBox(totalLength / 2f, 0.5f, new Vector2(totalLength / 2f, 0f), 0f);

        _fixture.TryCreateFixture(tape, shape, "tape_sensor", hard: false);

        _popup.PopupEntity(Loc.GetString("warning-tape-roll-placed"), args.User, args.User);
    }
}
