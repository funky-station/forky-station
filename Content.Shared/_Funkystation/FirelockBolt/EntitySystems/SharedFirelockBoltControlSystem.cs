using Content.Shared._Funkystation.FirelockBolt.Components;
using Content.Shared.DoAfter;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Content.Shared.Wires;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._Funkystation.FirelockBolt.EntitySystems;

public abstract partial class SharedFirelockBoltControlSystem : EntitySystem
{
    [Dependency] protected SharedDoorSystem DoorSystem = null!;
    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedUserInterfaceSystem _ui = null!;
    [Dependency] private IGameTiming _timing = null!;
    [Dependency] protected EntityQuery<DoorBoltComponent> DoorBoltQuery = default!;
    [Dependency] protected EntityQuery<DoorComponent> DoorQuery = default!;
    [Dependency] private EntityQuery<WiresPanelComponent> _wiresPanelQuery;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FirelockBoltControlComponent, DoorStateChangedEvent>(OnDoorStateChanged);
        SubscribeLocalEvent<FirelockBoltControlComponent, DoorBoltsChangedEvent>(OnBoltsChanged);
        SubscribeLocalEvent<FirelockBoltControlComponent, ActivateInWorldEvent>(OnActivate, before: new[] { typeof(SharedDoorSystem) });
        SubscribeLocalEvent<FirelockBoltControlComponent, InteractUsingEvent>(OnInteractUsing, before: new[] { typeof(SharedDoorSystem) });
        SubscribeLocalEvent<FirelockBoltControlComponent, FirelockOverrideToggleDoAfterEvent>(OnOverrideToggleDoAfter);
        SubscribeLocalEvent<FirelockBoltControlComponent, GetVerbsEvent<AlternativeVerb>>(OnGetVerbs);
        SubscribeLocalEvent<FirelockBoltControlComponent, BoundUIOpenedEvent>(OnBuiOpened);
        SubscribeLocalEvent<FirelockBoltControlComponent, FirelockOverrideSetMessage>(OnOverrideSetMessage);
    }

    private void OnDoorStateChanged(Entity<FirelockBoltControlComponent> ent, ref DoorStateChangedEvent args)
    {
        ApplyBoltForDoorState(ent, args.State);
        PushState(ent);
    }

    private void OnBoltsChanged(Entity<FirelockBoltControlComponent> ent, ref DoorBoltsChangedEvent args)
    {
        PushState(ent);
    }

    private void OnActivate(Entity<FirelockBoltControlComponent> ent, ref ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        // record interaction time to detect manual door closes
        ent.Comp.LastManualInteractionTime = _timing.CurTime;

        if (!_wiresPanelQuery.TryComp(ent.Owner, out var panel) || !panel.Open)
            return;

        // if the door is not bolted, don't intercept the click. let the door open normally
        var isBolted = DoorBoltQuery.TryComp(ent.Owner, out var bolt) && bolt.BoltsDown;
        if (!isBolted)
            return;

        args.Handled = true;
        _ui.OpenUi(ent.Owner, FirelockOverrideUiKey.Key, args.User);
    }

    private void OnInteractUsing(Entity<FirelockBoltControlComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        ent.Comp.LastManualInteractionTime = _timing.CurTime;
    }

    private void OnGetVerbs(Entity<FirelockBoltControlComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!_wiresPanelQuery.TryComp(ent.Owner, out var panel) || !panel.Open)
            return;

        var user = args.User;
        args.Verbs.Add(new AlternativeVerb
        {
            Text = Loc.GetString("firelock-bolt-control-verb-open-ui"),
            Act = () => _ui.OpenUi(ent.Owner, FirelockOverrideUiKey.Key, user),
            Icon = new SpriteSpecifier.Texture(new ("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"))
        });
    }

    private void OnBuiOpened(Entity<FirelockBoltControlComponent> ent, ref BoundUIOpenedEvent args)
    {
        if (args.UiKey is FirelockOverrideUiKey)
            PushState(ent);
    }

    protected void PushState(Entity<FirelockBoltControlComponent> ent)
    {
        var bolted = DoorBoltQuery.TryComp(ent.Owner, out var bolt) && bolt.BoltsDown;
        _ui.SetUiState(ent.Owner, FirelockOverrideUiKey.Key, new FirelockOverrideBuiState(ent.Comp.Override, bolted));
    }

    private void OnOverrideSetMessage(Entity<FirelockBoltControlComponent> ent, ref FirelockOverrideSetMessage args)
    {
        if (ent.Comp.Override == args.Override)
            return;

        var doAfterArgs = new DoAfterArgs(EntityManager,
            args.Actor,
            ent.Comp.ToggleDelay,
            new FirelockOverrideToggleDoAfterEvent(args.Override),
            ent.Owner,
            target: ent.Owner)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnOverrideToggleDoAfter(Entity<FirelockBoltControlComponent> ent, ref FirelockOverrideToggleDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        args.Handled = true;
        SetOverride(ent, args.TargetOverride);
    }

    private void ApplyBoltForDoorState(Entity<FirelockBoltControlComponent> ent, DoorState state)
    {
        if (ent.Comp.Override || !DoorBoltQuery.TryComp(ent.Owner, out var bolt))
            return;

        switch (state)
        {
            case DoorState.Closing:
                // if the door starts closing within 1.5 seconds of a player clicking it, it's a manual close, don't bolt
                ent.Comp.IsManualClose = (_timing.CurTime - ent.Comp.LastManualInteractionTime) < TimeSpan.FromSeconds(1.5);
                break;

            case DoorState.Closed:
            case DoorState.Welded:
                // bolt shut if it was automatically closed
                if (ent.Comp.AlarmActive || !ent.Comp.IsManualClose)
                    DoorSystem.SetBoltsDown((ent.Owner, bolt), true, predicted: true);
                break;

            case DoorState.Open:
                DoorSystem.SetBoltsDown((ent.Owner, bolt), false, predicted: true);
                ent.Comp.IsManualClose = false;
                break;
        }
    }

    private void SetOverride(Entity<FirelockBoltControlComponent> ent, bool value)
    {
        if (ent.Comp.Override == value)
            return;

        ent.Comp.Override = value;
        Dirty(ent, ent.Comp);

        var sound = value ? ent.Comp.EnableSound : ent.Comp.DisableSound;
        _audio.PlayPvs(sound, ent.Owner);

        if (value)
        {
            if (DoorBoltQuery.TryComp(ent.Owner, out var bolt))
                DoorSystem.SetBoltsDown((ent.Owner, bolt), false);
        }
        else if (DoorQuery.TryComp(ent.Owner, out var door))
        {
            ApplyBoltForDoorState(ent, door.State);
        }

        PushState(ent);
    }
}
