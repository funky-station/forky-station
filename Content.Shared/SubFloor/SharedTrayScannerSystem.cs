using Content.Shared._Funkystation.Clothing.Components; // Funky change
using Content.Shared.Eye;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Actions; // Funky change
using Content.Shared.Actions.Components; // Funky change
using Robust.Shared.Audio.Systems; // Funky change
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.SubFloor;

public abstract class SharedTrayScannerSystem : EntitySystem
{
    [Dependency] private readonly INetManager _netMan = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedEyeSystem _eye = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!; // Funky change
    [Dependency] private readonly SharedAudioSystem _audio = default!; // Funky change
    public const float SubfloorRevealAlpha = 0.8f;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TrayScannerComponent, ComponentGetState>(OnTrayScannerGetState);
        SubscribeLocalEvent<TrayScannerComponent, ComponentHandleState>(OnTrayScannerHandleState);
        SubscribeLocalEvent<TrayScannerComponent, ActivateInWorldEvent>(OnTrayScannerActivate);
        SubscribeLocalEvent<TrayScannerComponent, ToggleTrayScannerEvent>(OnToggleAction); // Funky change

        SubscribeLocalEvent<TrayScannerComponent, GotEquippedHandEvent>(OnTrayHandEquipped);
        SubscribeLocalEvent<TrayScannerComponent, GotUnequippedHandEvent>(OnTrayHandUnequipped);
        SubscribeLocalEvent<TrayScannerComponent, GotEquippedEvent>(OnTrayEquipped);
        SubscribeLocalEvent<TrayScannerComponent, GotUnequippedEvent>(OnTrayUnequipped);

        SubscribeLocalEvent<TrayScannerUserComponent, GetVisMaskEvent>(OnUserGetVis);
    }

    private void OnUserGetVis(Entity<TrayScannerUserComponent> ent, ref GetVisMaskEvent args)
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
        OnUnequip(args.Equipee);

        // Funky change
        if (ent.Comp.ToggleActionEntity is { } action)
        {
            if (TryComp(action, out TransformComponent? xform) && xform.ParentUid == args.Equipee)
                _actions.RemoveAction(args.Equipee, action);
            else
                QueueDel(action);

            ent.Comp.ToggleActionEntity = null;
        }
    }

    private void OnTrayEquipped(Entity<TrayScannerComponent> ent, ref GotEquippedEvent args)
    {
        OnEquip(args.Equipee);

        // Funky change
        if (ent.Comp.ToggleAction != null && HasComp<ActionsComponent>(args.Equipee))
            _actions.AddAction(args.Equipee, ref ent.Comp.ToggleActionEntity, ent.Comp.ToggleAction.Value, ent);
    }

    // Funky change
    private void OnToggleAction(EntityUid uid, TrayScannerComponent scanner, ToggleTrayScannerEvent args)
    {
        if (args.Handled)
            return;

        ToggleScanner(uid, args.Performer, scanner);
        args.Handled = true;
    }

    private void OnTrayScannerActivate(EntityUid uid, TrayScannerComponent scanner, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        ToggleScanner(uid, args.User, scanner); // Funky change
        args.Handled = true;
    }

    // Funky change
    private void ToggleScanner(EntityUid uid, EntityUid user, TrayScannerComponent scanner)
    {
        var isEnabled = !scanner.Enabled;
        SetScannerEnabled(uid, isEnabled, scanner);

        var sound = isEnabled ? scanner.SoundOn : scanner.SoundOff;
        _audio.PlayPredicted(sound, uid, user);
    }

    private void SetScannerEnabled(EntityUid uid, bool enabled, TrayScannerComponent? scanner = null)
    {
        if (!Resolve(uid, ref scanner) || scanner.Enabled == enabled)
            return;

        scanner.Enabled = enabled;
        Dirty(uid, scanner);

        // Funky change
        if (TryComp(uid, out GoggleShaderComponent? goggleShader))
        {
            goggleShader.Enabled = enabled;
            Dirty(uid, goggleShader);

            var ev = new GoggleShaderToggledEvent(enabled);
            RaiseLocalEvent(uid, ref ev);
        }

        // We don't remove from _activeScanners on disabled, because the update function will handle that, as well as
        // managing the revealed subfloor entities

        if (TryComp(uid, out AppearanceComponent? appearance))
        {
            _appearance.SetData(uid, TrayScannerVisual.Visual, scanner.Enabled ? TrayScannerVisual.On : TrayScannerVisual.Off, appearance);
        }
    }

    private void OnTrayScannerGetState(EntityUid uid, TrayScannerComponent scanner, ref ComponentGetState args)
    {
        args.State = new TrayScannerState(scanner.Enabled, scanner.Range);
    }

    private void OnTrayScannerHandleState(EntityUid uid, TrayScannerComponent scanner, ref ComponentHandleState args)
    {
        if (args.Current is not TrayScannerState state)
            return;

        scanner.Range = state.Range;
        SetScannerEnabled(uid, state.Enabled, scanner);
    }
}

[Serializable, NetSerializable]
public enum TrayScannerVisual : sbyte
{
    Visual,
    On,
    Off
}
