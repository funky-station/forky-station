using Content.Server._Funkystation.DeviceLinking.Components;
using Content.Server._Funkystation.SignalKey;
using Content.Server.DeviceLinking.Systems;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server._Funkystation.DeviceLinking.Systems;

public sealed partial class SignalLockSystem : EntitySystem
{
    [Dependency] private DeviceLinkSystem _deviceLink = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private LockSystem _lock = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SignalLockComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<SignalLockComponent, InteractUsingEvent>(OnInteractUsing);
    }

    private void OnInit(EntityUid uid, SignalLockComponent comp, ComponentInit args)
    {
        _deviceLink.EnsureSourcePorts(uid, comp.Port);
    }

    private void OnInteractUsing(EntityUid uid, SignalLockComponent comp, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (_lock.IsLocked(uid))
            return;

        if (!TryComp<SignalKeyComponent>(args.Used, out var key))
            return;

        var audioParams = AudioParams.Default.AddVolume(8f);

        if (comp.KeyType == key.KeyType)
        {
            _deviceLink.InvokePort(uid, comp.Port);
            _audio.PlayPvs(comp.LockSound, uid, audioParams);
        }
        else
        {
            _audio.PlayPvs(comp.WrongKeySound, uid, audioParams);
            _popup.PopupEntity(Loc.GetString("fu-signal-lock-wrong-key"), uid);
        }

        args.Handled = true;
    }
}
