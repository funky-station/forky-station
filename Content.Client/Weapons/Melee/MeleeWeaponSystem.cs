// SPDX-FileCopyrightText: 2019, 2022, 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2019 ZelteHonor <gabrieldionbouchard@gmail.com>
// SPDX-FileCopyrightText: 2020-2021, 2023-2024 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020-2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <zddm@outlook.es>
// SPDX-FileCopyrightText: 2020 Vince <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 Metal Gear Sloth <metalgearsloth@gmail.com>
// SPDX-FileCopyrightText: 2020 Clyybber <darkmine956@gmail.com>
// SPDX-FileCopyrightText: 2021-2025 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 mirrorcult <notzombiedude@gmail.com>
// SPDX-FileCopyrightText: 2022-2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023-2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023-2024 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 I.K <45953835+notquitehadouken@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Vordenburg <114301317+Vordenburg@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2024 Джексон Миссиссиппи <tripwiregamer@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Magnus Larsen <i.am.larsenml@gmail.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 Sigil <84070966+Siigiil@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 SlamBamActionman <83650252+SlamBamActionman@users.noreply.github.com>
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Client.Gameplay;
using Content.Shared.CombatMode;
using Content.Shared.Effects;
using Content.Shared.Hands.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusEffect;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client.Weapons.Melee;

public sealed partial class MeleeWeaponSystem : SharedMeleeWeaponSystem
{
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IStateManager _stateManager = default!;
    [Dependency] private readonly AnimationPlayerSystem _animation = default!;
    [Dependency] private readonly InputSystem _inputSystem = default!;
    [Dependency] private readonly SharedColorFlashEffectSystem _color = default!;
    [Dependency] private readonly MapSystem _map = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private EntityQuery<TransformComponent> _xformQuery;

    private const string MeleeLungeKey = "melee-lunge";

    public override void Initialize()
    {
        base.Initialize();
        _xformQuery = GetEntityQuery<TransformComponent>();
        SubscribeNetworkEvent<MeleeLungeEvent>(OnMeleeLunge);
        UpdatesOutsidePrediction = true;
    }

    public override void FrameUpdate(float frameTime)
    {
        base.FrameUpdate(frameTime);
        UpdateEffects();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!Timing.IsFirstTimePredicted)
            return;

        var entityNull = _player.LocalEntity;

        if (entityNull == null)
            return;

        var entity = entityNull.Value;

        if (!TryGetWeapon(entity, out var weaponUid, out var weapon))
            return;

        if (!CombatMode.IsInCombatMode(entity) || !Blocker.CanAttack(entity, weapon: (weaponUid, weapon)))
        {
            weapon.Attacking = false;
            return;
        }

        var useDown = _inputSystem.CmdStates.GetState(EngineKeyFunctions.Use);
        var altDown = _inputSystem.CmdStates.GetState(EngineKeyFunctions.UseSecondary);

        if (weapon.AutoAttack || useDown != BoundKeyState.Down && altDown != BoundKeyState.Down)
        {
            if (weapon.Attacking)
            {
                RaisePredictiveEvent(new StopAttackEvent(GetNetEntity(weaponUid)));
            }
        }

        if (weapon.Attacking || weapon.NextAttack > Timing.CurTime)
        {
            return;
        }

        // TODO using targeted actions while combat mode is enabled should NOT trigger attacks.

        var mousePos = _eyeManager.PixelToMap(_inputManager.MouseScreenPosition);

        if (mousePos.MapId == MapId.Nullspace)
        {
            return;
        }

        EntityCoordinates coordinates;

        if (MapManager.TryFindGridAt(mousePos, out var gridUid, out _))
        {
            coordinates = TransformSystem.ToCoordinates(gridUid, mousePos);
        }
        else
        {
            coordinates = TransformSystem.ToCoordinates(_map.GetMap(mousePos.MapId), mousePos);
        }

        // If the gun has AltFireComponent, it can be used to attack.
        if (TryComp<GunComponent>(weaponUid, out var gun) && gun.UseKey)
        {
            if (!TryComp<AltFireMeleeComponent>(weaponUid, out var altFireComponent) || altDown != BoundKeyState.Down)
                return;

            switch(altFireComponent.AttackType)
            {
                case AltFireAttackType.Light:
                    ClientLightAttack(entity, mousePos, coordinates, weaponUid, weapon);
                    break;

                case AltFireAttackType.Heavy:
                    ClientHeavyAttack(entity, coordinates, weaponUid, weapon);
                    break;

                case AltFireAttackType.Disarm:
                    ClientDisarm(entity, mousePos, coordinates);
                    break;
            }

            return;
        }

        // Heavy attack.
        if (altDown == BoundKeyState.Down)
        {
            // If it's an unarmed attack then do a disarm
            if (weapon.AltDisarm && weaponUid == entity)
            {
                ClientDisarm(entity, mousePos, coordinates);
                return;
            }

            ClientHeavyAttack(entity, coordinates, weaponUid, weapon);
            return;
        }

        // Light attack
        if (useDown == BoundKeyState.Down)
            ClientLightAttack(entity, mousePos, coordinates, weaponUid, weapon);
    }

    protected override bool InRange(EntityUid user, EntityUid target, float range, ICommonSession? session)
    {
        var xform = Transform(target);
        var targetCoordinates = xform.Coordinates;
        var targetLocalAngle = xform.LocalRotation;

        return Interaction.InRangeUnobstructed(user, target, targetCoordinates, targetLocalAngle, range, overlapCheck: false);
    }

    protected override void DoDamageEffect(List<EntityUid> targets, EntityUid? user, TransformComponent targetXform)
    {
        // Server never sends the event to us for predictiveeevent.
        _color.RaiseEffect(Color.Red, targets, Filter.Local());
    }

    /// <summary>
    /// Raises a heavy attack event with the relevant attacked entities.
    /// This is to avoid lag effecting the client's perspective too much.
    /// </summary>
    private void ClientHeavyAttack(EntityUid user, EntityCoordinates coordinates, EntityUid meleeUid, MeleeWeaponComponent component)
    {
        // Only run on first prediction to avoid the potential raycast entities changing.
        if (!_xformQuery.TryGetComponent(user, out var userXform) ||
            !Timing.IsFirstTimePredicted)
        {
            return;
        }

        var targetMap = TransformSystem.ToMapCoordinates(coordinates);

        if (targetMap.MapId != userXform.MapID)
            return;

        var userPos = TransformSystem.GetWorldPosition(userXform);
        var direction = targetMap.Position - userPos;
        var distance = MathF.Min(component.Range, direction.Length());

        // This should really be improved. GetEntitiesInArc uses pos instead of bounding boxes.
        // Server will validate it with InRangeUnobstructed.
        var entities = GetNetEntityList(ArcRayCast(userPos, direction.ToWorldAngle(), component.Angle, distance, userXform.MapID, user).ToList());
        RaisePredictiveEvent(new HeavyAttackEvent(GetNetEntity(meleeUid), entities.GetRange(0, Math.Min(MaxTargets, entities.Count)), GetNetCoordinates(coordinates)));
    }

    private void ClientDisarm(EntityUid attacker, MapCoordinates mousePos, EntityCoordinates coordinates)
    {
        EntityUid? target = null;

        if (_stateManager.CurrentState is GameplayStateBase screen)
            target = screen.GetClickedEntity(mousePos);

        RaisePredictiveEvent(new DisarmAttackEvent(GetNetEntity(target), GetNetCoordinates(coordinates)));
    }

    private void ClientLightAttack(EntityUid attacker, MapCoordinates mousePos, EntityCoordinates coordinates, EntityUid weaponUid, MeleeWeaponComponent meleeComponent)
    {
        var attackerPos = TransformSystem.GetMapCoordinates(attacker);

        if (mousePos.MapId != attackerPos.MapId || (attackerPos.Position - mousePos.Position).Length() > meleeComponent.Range)
            return;

        EntityUid? target = null;

        if (_stateManager.CurrentState is GameplayStateBase screen)
            target = screen.GetClickedEntity(mousePos);

        // Don't light-attack if interaction will be handling this instead
        if (Interaction.CombatModeCanHandInteract(attacker, target))
            return;

        RaisePredictiveEvent(new LightAttackEvent(GetNetEntity(target), GetNetEntity(weaponUid), GetNetCoordinates(coordinates)));
    }

    private void OnMeleeLunge(MeleeLungeEvent ev)
    {
        var ent = GetEntity(ev.Entity);
        var entWeapon = GetEntity(ev.Weapon);

        // Entity might not have been sent by PVS.
        if (Exists(ent) && Exists(entWeapon))
            DoLunge(ent, entWeapon, ev.Angle, ev.LocalPos, ev.Animation);
    }
}
