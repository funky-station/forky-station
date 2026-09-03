using Content.Shared._Funkystation.WarningTape.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._Funkystation.WarningTape;

public sealed partial class SharedTapeSystem : EntitySystem
{
    private static readonly ProtoId<ToolQualityPrototype> QualityCutting = "Cutting";
    private static readonly ProtoId<ToolQualityPrototype> QualitySlicing = "Slicing";

    [Dependency] private SharedDoAfterSystem _doAfter = null!;
    [Dependency] private SharedPopupSystem _popup = null!;
    [Dependency] private SharedAudioSystem _audio = null!;
    [Dependency] private SharedToolSystem _tool = null!;
    [Dependency] private INetManager _net = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TapeComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<TapeComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<TapeComponent, TapeRemoveDoAfterEvent>(OnRemoveDoAfter);
    }

    private void OnInteractHand(Entity<TapeComponent> ent, ref InteractHandEvent args)
    {
        if (args.Handled)
            return;

        StartDoAfter(ent, args.User, ent.Comp.RemoveDelay);
        args.Handled = true;
    }

    // cuts through the tape instantly if using a cutting/slicing tool
    private void OnInteractUsing(Entity<TapeComponent> ent, ref InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (!_tool.HasQuality(args.Used, QualityCutting) && !_tool.HasQuality(args.Used, QualitySlicing))
            return;

        args.Handled = true;
        _popup.PopupPredicted(Loc.GetString("warning-tape-removed"), args.User, args.User);
        _audio.PlayPredicted(ent.Comp.RemoveSound, args.User, args.User);

        if (_net.IsServer)
            QueueDel(ent.Owner);
    }

    private void StartDoAfter(Entity<TapeComponent> ent, EntityUid user, TimeSpan delay)
    {
        var doAfterArgs = new DoAfterArgs(EntityManager,
            user,
            delay,
            new TapeRemoveDoAfterEvent(),
            ent.Owner,
            target: null,
            used: null)
        {
            BreakOnMove = true,
            NeedHand = true,
            BreakOnDamage = true,
            DistanceThreshold = 4.0f,
        };

        _doAfter.TryStartDoAfter(doAfterArgs);
    }

    private void OnRemoveDoAfter(Entity<TapeComponent> ent, ref TapeRemoveDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        args.Handled = true;

        _popup.PopupPredicted(Loc.GetString("warning-tape-removed"), args.User, args.User);
        _audio.PlayPredicted(ent.Comp.RemoveSound, args.User, args.User);

        if (_net.IsServer)
            QueueDel(ent.Owner);
    }
}
