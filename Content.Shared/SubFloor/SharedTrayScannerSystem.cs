using Content.Shared._Funkystation.Clothing.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Database;
using Content.Shared.Eye;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
// Funky change
// Funky change
// Funky change
// Funky change

namespace Content.Shared.SubFloor;

public abstract partial class SharedTrayScannerSystem : EntitySystem
{
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedEyeSystem _eye = default!;
    [Dependency] private UseDelaySystem _delay = default!;
    [Dependency] private INetManager _netMan = default!;
    [Dependency] private SharedActionsSystem _actions = default!; // Funky change

    public const float SubfloorRevealAlpha = 0.8f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TrayScannerComponent, ActivateInWorldEvent>(OnTrayScannerActivate);
        SubscribeLocalEvent<TrayScannerComponent, GetVerbsEvent<AlternativeVerb>>(OnAddSwitchModeVerb);
        SubscribeLocalEvent<TrayScannerComponent, ToggleTrayScannerEvent>(OnToggleAction); // Funky change

        SubscribeLocalEvent<TrayScannerComponent, GotEquippedHandEvent>(OnTrayHandEquipped);
        SubscribeLocalEvent<TrayScannerComponent, GotUnequippedHandEvent>(OnTrayHandUnequipped);
        SubscribeLocalEvent<TrayScannerComponent, GotEquippedEvent>(OnTrayEquipped);
        SubscribeLocalEvent<TrayScannerComponent, GotUnequippedEvent>(OnTrayUnequipped);
        SubscribeLocalEvent<TrayScannerUserComponent, GetVisMaskEvent>(OnUserGetVis);
    }

    private void OnAddSwitchModeVerb(Entity<TrayScannerComponent> scanner, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || !args.Using.HasValue || !scanner.Comp.Enabled)
            return;

        var user = args.User;

        AlternativeVerb verb = new()
        {
            Text = Loc.GetString("tray-scanner-switch-mode"),
            Icon = new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png")),
            Act = () => SwitchMode(scanner, user),
            Impact = LogImpact.Low
        };
        args.Verbs.Add(verb);
    }

    private static TrayScannerMode Next(TrayScannerMode mode)
    {
        return mode switch
        {
            TrayScannerMode.All => TrayScannerMode.Wiring,
            TrayScannerMode.Wiring => TrayScannerMode.Piping,
            TrayScannerMode.Piping => TrayScannerMode.All,
            _ => TrayScannerMode.All,
        };
    }

    private void SwitchMode(Entity<TrayScannerComponent> scanner, EntityUid? userUid)
    {
        if (!userUid.HasValue)
            return;

        // Prevents ping spam
        if (!_delay.TryResetDelay(scanner, checkDelayed: true))
            return;

        scanner.Comp.Mode = Next(scanner.Comp.Mode);
        Dirty(scanner);

        // Play a slightly different sound when we're back to All mode
        var pitch = scanner.Comp.Mode == TrayScannerMode.All ? 1 : 0.8f;
        _audio.PlayPredicted(scanner.Comp.SoundSwitchMode, scanner, userUid, AudioParams.Default.WithVolume(1.5f).WithPitchScale(pitch));
    }

    private void OnUserGetVis(Entity<TrayScannerUserComponent> scanner, ref GetVisMaskEvent args)
    {
        args.VisibilityMask |= (int)VisibilityFlags.Subfloor;
    }

    private void OnEquip(EntityUid user)
    {
        if (_netMan.IsClient)
            return;

        var comp = EnsureComp<TrayScannerUserComponent>(user);
        comp.Count++;

        if (comp.Count > 1)
            return;

        _eye.RefreshVisibilityMask(user);
    }

    private void OnUnequip(EntityUid user)
    {
        if (_netMan.IsClient)
            return;

        if (!TryComp(user, out TrayScannerUserComponent? comp))
            return;

        comp.Count--;

        if (comp.Count > 0)
            return;

        RemComp<TrayScannerUserComponent>(user);
        _eye.RefreshVisibilityMask(user);
    }

    private void OnTrayHandUnequipped(Entity<TrayScannerComponent> ent, ref GotUnequippedHandEvent args)
    {
        OnUnequip(args.User);

        // Funky change
        if (ent.Comp.ToggleActionEntity is { } action)
        {
            if (TryComp(action, out TransformComponent? xform) && xform.ParentUid == args.User)
                _actions.RemoveAction(args.User, action);
            else
                QueueDel(action);

            ent.Comp.ToggleActionEntity = null;
        }
    }

    private void OnTrayHandEquipped(Entity<TrayScannerComponent> ent, ref GotEquippedHandEvent args)
    {
        OnEquip(args.User);

        // Funky change
        if (ent.Comp.ToggleAction != null && HasComp<ActionsComponent>(args.User))
            _actions.AddAction(args.User, ref ent.Comp.ToggleActionEntity, ent.Comp.ToggleAction.Value, ent);
    }

    private void OnTrayUnequipped(Entity<TrayScannerComponent> ent, ref GotUnequippedEvent args)
    {
        OnUnequip(args.EquipTarget);

        // Funky change
        if (ent.Comp.ToggleActionEntity is { } action)
        {
            if (TryComp(action, out TransformComponent? xform) && xform.ParentUid == args.EquipTarget)
                _actions.RemoveAction(args.EquipTarget, action);
            else
                QueueDel(action);

            ent.Comp.ToggleActionEntity = null;
        }
    }

    private void OnTrayEquipped(Entity<TrayScannerComponent> ent, ref GotEquippedEvent args)
    {
        OnEquip(args.EquipTarget);

        // Funky change
        if (ent.Comp.ToggleAction != null && HasComp<ActionsComponent>(args.EquipTarget))
            _actions.AddAction(args.EquipTarget, ref ent.Comp.ToggleActionEntity, ent.Comp.ToggleAction.Value, ent);
    }

    // Funky change
    private void OnToggleAction(EntityUid uid, TrayScannerComponent scanner, ToggleTrayScannerEvent args)
    {
        if (args.Handled)
            return;

        ToggleScanner((uid, scanner), args.Performer);
        args.Handled = true;
    }

    private void OnTrayScannerActivate(Entity<TrayScannerComponent> ent, ref ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        ToggleScanner(ent, args.User); // Funky change
        args.Handled = true;
    }

    // Funky change
    private void ToggleScanner(Entity<TrayScannerComponent> ent, EntityUid user)
    {
        var isEnabled = !ent.Comp.Enabled;
        SetScannerEnabled(ent, isEnabled);

        var sound = isEnabled ? ent.Comp.SoundOn : ent.Comp.SoundOff;
        _audio.PlayPredicted(sound, ent, user);
    }

    private void SetScannerEnabled(Entity<TrayScannerComponent> ent, bool enabled)
    {
        if (ent.Comp.Enabled == enabled)
            return;

        ent.Comp.Enabled = enabled;
        Dirty(ent);

        // Funky change
        if (TryComp(ent, out GoggleShaderComponent? goggleShader))
        {
            goggleShader.Enabled = enabled;
            Dirty(ent, goggleShader);

            var ev = new GoggleShaderToggledEvent(enabled);
            RaiseLocalEvent(ent, ref ev);
        }

        // We don't remove from _activeScanners on disabled, because the update function will handle that, as well as
        // managing the revealed subfloor entities

        if (TryComp(ent, out AppearanceComponent? appearance))
        {
            _appearance.SetData(ent, TrayScannerVisual.Visual, ent.Comp.Enabled ? TrayScannerVisual.On : TrayScannerVisual.Off, appearance);
        }
    }
}

[Serializable, NetSerializable]
public enum TrayScannerVisual : sbyte
{
    Visual,
    On,
    Off
}
