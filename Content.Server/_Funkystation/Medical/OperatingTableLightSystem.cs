using Content.Shared._Funkystation.Medical;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Utility;

namespace Content.Server._Funkystation.Medical;

public sealed class OperatingTableLightSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly MobThresholdSystem _mobThresholdSystem = default!;
    [Dependency] private readonly SharedPointLightSystem _pointLight = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<OperatingTableLightComponent, GetVerbsEvent<AlternativeVerb>>(OnGetVerbs);
        SubscribeLocalEvent<OperatingTableLightComponent, StrappedEvent>(OnStrapped);
        SubscribeLocalEvent<OperatingTableLightComponent, UnstrappedEvent>(OnUnstrapped);

        // Subscribe to health change events for live vitals updates
        SubscribeLocalEvent<DamageableComponent, DamageChangedEvent>(OnDamageChanged);
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnGetVerbs(EntityUid uid, OperatingTableLightComponent comp, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanInteract || !args.CanAccess)
            return;

        args.Verbs.Add(new AlternativeVerb
        {
            Text = comp.LightOn ? "Turn Off Light" : "Turn On Light",
            Act = () => ToggleLight(uid, comp),
            Priority = -1,
            Icon = new SpriteSpecifier.Texture(new ("/Textures/Interface/VerbIcons/light.svg.192dpi.png"))
        });
    }

    private void ToggleLight(EntityUid uid, OperatingTableLightComponent comp)
    {
        comp.LightOn = !comp.LightOn;
        _appearance.SetData(uid, OperatingTableVisuals.LightOn, comp.LightOn);
        _audio.PlayPvs("/Audio/Machines/button.ogg", uid);

        if (TryComp<PointLightComponent>(uid, out var pointLight))
            _pointLight.SetEnabled(uid, comp.LightOn, pointLight);

        Dirty(uid, comp);
    }

    private void OnStrapped(EntityUid uid, OperatingTableLightComponent comp, ref StrappedEvent args)
    {
        var patient = args.Buckle.Owner;
        UpdateVitalsDisplay(uid, patient);

        if (TryComp<MobStateComponent>(patient, out var mobState))
        {
            if (_mobState.IsDead(patient, mobState))
            {
                PlayFlatline(uid, comp);
            }
            else
            {
                StartHeartbeat(uid, comp, patient);
            }
        }
    }

    private void OnUnstrapped(EntityUid uid, OperatingTableLightComponent comp, ref UnstrappedEvent args)
    {
        if (!TryComp<StrapComponent>(uid, out var strap) || strap.BuckledEntities.Count == 0)
        {
            _appearance.SetData(uid, OperatingTableVisuals.VitalsState, VitalsState.None);

            StopHeartbeat(uid, comp);
            StopFlatline(uid, comp);
        }
    }

    private void UpdateVitalsDisplay(EntityUid table, EntityUid patient)
    {
        if (!TryComp<MobStateComponent>(patient, out var mobState))
        {
            _appearance.SetData(table, OperatingTableVisuals.VitalsState, VitalsState.None);
            return;
        }

        VitalsState state;

        if (_mobState.IsDead(patient, mobState))
        {
            state = VitalsState.Dead;
        }
        else if (_mobState.IsCritical(patient, mobState))
        {
            state = VitalsState.Critical;
        }
        else if (HasComp<DamageableComponent>(patient))
        {
            var totalHealth = _damageable.GetTotalDamage(patient);
            var maxHealth = 100f;

            if (_mobThresholdSystem.TryGetThresholdForState(patient, MobState.Critical, out var critThreshold))
                maxHealth = critThreshold.Value.Float();

            var healthPercent = 1.0f - (totalHealth.Float() / maxHealth);

            if (healthPercent > 0.85f)
                state = VitalsState.Healthy;
            else if (healthPercent > 0.5f)
                state = VitalsState.Injured;
            else
                state = VitalsState.Critical;
        }
        else
        {
            state = VitalsState.Healthy;
        }

        _appearance.SetData(table, OperatingTableVisuals.VitalsState, state);
    }

    private SoundSpecifier? GetHeartbeatSoundForPatient(EntityUid uid, OperatingTableLightComponent comp, EntityUid patient)
    {
        if (!TryComp<MobStateComponent>(patient, out var mobState))
            return comp.HeartbeatHealthySound;

        if (_mobState.IsDead(patient, mobState))
            return null;

        if (_mobState.IsCritical(patient, mobState))
            return comp.HeartbeatCriticalSound;

        if (HasComp<DamageableComponent>(patient))
        {
            var totalHealth = _damageable.GetTotalDamage(patient).Float();
            var maxHealth = 100f;

            if (_mobThresholdSystem.TryGetThresholdForState(patient, MobState.Critical, out var critThreshold))
                maxHealth = critThreshold.Value.Float();

            var healthPercent = 1.0f - (totalHealth / maxHealth);

            if (healthPercent > 0.85f)
                return comp.HeartbeatHealthySound;
            else if (healthPercent > 0.5f)
                return comp.HeartbeatInjuredSound;
            else
                return comp.HeartbeatCriticalSound;
        }

        return comp.HeartbeatHealthySound;
    }

    private void UpdateHeartbeatPitch(EntityUid uid, OperatingTableLightComponent comp, EntityUid patient)
    {
        var targetSound = GetHeartbeatSoundForPatient(uid, comp, patient);

        if (targetSound != comp.CurrentHeartbeatSound)
        {
            StartHeartbeat(uid, comp, patient);
        }
    }

    private void OnDamageChanged(EntityUid uid, DamageableComponent component, DamageChangedEvent args)
    {
        if (!TryComp<BuckleComponent>(uid, out var buckle))
            return;

        if (buckle.BuckledTo == null)
            return;

        var table = buckle.BuckledTo.Value;

        if (!TryComp<OperatingTableLightComponent>(table, out var tableComp))
            return;

        UpdateVitalsDisplay(table, uid);

        if (TryComp<MobStateComponent>(uid, out var mobState))
        {
            if (_mobState.IsDead(uid, mobState))
            {
                PlayFlatline(table, tableComp);
            }
            else if (tableComp.HeartbeatStream == null)
            {
                StopFlatline(table, tableComp);
                StartHeartbeat(table, tableComp, uid);
            }
            else
            {
                UpdateHeartbeatPitch(table, tableComp, uid);
            }
        }
    }

    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        var uid = args.Target;

        if (!TryComp<BuckleComponent>(uid, out var buckle))
            return;

        if (buckle.BuckledTo == null)
            return;

        var table = buckle.BuckledTo.Value;

        if (!TryComp<OperatingTableLightComponent>(table, out var tableComp))
            return;

        UpdateVitalsDisplay(table, uid);

        if (TryComp<MobStateComponent>(uid, out var mobState))
        {
            if (_mobState.IsDead(uid, mobState))
            {
                PlayFlatline(table, tableComp);
            }
            else if (tableComp.HeartbeatStream == null)
            {
                StopFlatline(table, tableComp);
                StartHeartbeat(table, tableComp, uid);
            }
            else
            {
                UpdateHeartbeatPitch(table, tableComp, uid);
            }
        }
    }

    private void StartHeartbeat(EntityUid uid, OperatingTableLightComponent comp, EntityUid patient)
    {
        StopHeartbeat(uid, comp);

        var heartbeatSound = GetHeartbeatSoundForPatient(uid, comp, patient);
        if (heartbeatSound == null)
            return;

        var stream = _audio.PlayPvs(
            heartbeatSound,
            uid,
            AudioParams.Default.WithLoop(true).WithVolume(-5f)
        );

        if (stream != null)
        {
            comp.HeartbeatStream = stream.Value.Entity;
            comp.CurrentHeartbeatSound = heartbeatSound;
        }
    }

    private void StopHeartbeat(EntityUid uid, OperatingTableLightComponent comp)
    {
        if (comp.HeartbeatStream != null)
        {
            _audio.Stop(comp.HeartbeatStream.Value);
            comp.HeartbeatStream = null;
            comp.CurrentHeartbeatSound = null;
        }
    }

    private void StopFlatline(EntityUid uid, OperatingTableLightComponent comp)
    {
        if (comp.FlatlineStream != null)
        {
            _audio.Stop(comp.FlatlineStream.Value);
            comp.FlatlineStream = null;
        }
    }

    private void PlayFlatline(EntityUid uid, OperatingTableLightComponent comp)
    {
        StopHeartbeat(uid, comp);
        StopFlatline(uid, comp);

        if (comp.FlatlineSound != null)
        {
            var stream = _audio.PlayPvs(
                comp.FlatlineSound,
                uid,
                AudioParams.Default.WithLoop(true).WithVolume(-3f)
            );

            if (stream != null)
            {
                comp.FlatlineStream = stream.Value.Entity;
            }
        }
    }
}
